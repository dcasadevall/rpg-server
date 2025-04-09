using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RPGCharacterService.Dtos.Currency.Requests;
using RPGCharacterService.Dtos.Currency.Responses;
using RPGCharacterService.Exceptions.Character;
using RPGCharacterService.Exceptions.Currency;
using RPGCharacterService.Entities;
using RPGCharacterService.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace RPGCharacterService.Controllers {
  /// <summary>
  /// Controller responsible for managing character currency operations.
  /// Handles initialization, modification, and exchange of different currency types (gold, silver, copper, etc.).
  /// </summary>
  [ApiController]
  [ApiVersion("1.0")]
  [Route("api/v{version:ApiVersion}/characters/{characterId:guid}/currency")]
  public class CurrencyController(ICurrencyService currencyService, IMapper mapper)
    : ControllerBase {
    /// <summary>
    /// Initializes a character's currency by randomly generating starting amounts.
    /// This operation can only be performed once per character.
    /// </summary>
    /// <param name="characterId">The unique identifier of the character.</param>
    /// <returns>The initialized currency amounts for the character.</returns>
    [HttpPost("init")]
    [SwaggerOperation(Summary = "Initialize a Character's Currency",
                       Description = "Randomly sets the character's starting currency by rolling dice")]
    [SwaggerResponse(200, "Currency Initialized", typeof(CurrencyResponse))]
    [SwaggerResponse(400, "Invalid ID Format")]
    [SwaggerResponse(404, "Character Not Found")]
    [SwaggerResponse(409, "Currency Already Initialized")]
    public async Task<ActionResult<CurrencyResponse>> InitializeCurrency(
      [SwaggerParameter("Character identifier", Required = true)] Guid characterId) {
      try {
        var character = await currencyService.GenerateInitialCurrencyAsync(characterId);
        var currencyResponse = mapper.Map<CurrencyResponse>(character.Wealth);
        return Ok(currencyResponse);
      } catch (CharacterNotFoundException) {
        return NotFound(new {
          error = "CHARACTER_NOT_FOUND",
          message = "Character not found."
        });
      } catch (CurrencyAlreadyInitializedException ex) {
        return Conflict(new {
          error = "CURRENCY_ALREADY_INITIALIZED",
          message = ex.Message
        });
      }
    }

    /// <summary>
    /// Modifies a character's currency by adding or subtracting specified amounts.
    /// Allows for changes to multiple currency types in a single operation.
    /// </summary>
    /// <param name="characterId">The unique identifier of the character.</param>
    /// <param name="request">The currency modification details including amounts to add or subtract.</param>
    /// <returns>The updated currency amounts for the character.</returns>
    [HttpPatch]
    [SwaggerOperation(Summary = "Modify Character Currency",
                       Description = "Adds or subtracts the specified values from the character's current currency")]
    [SwaggerResponse(200, "Currency Modified", typeof(CurrencyResponse))]
    [SwaggerResponse(400, "Invalid Request")]
    [SwaggerResponse(404, "Character Not Found")]
    [SwaggerResponse(409, "Currency Not Initialized")]
    [SwaggerResponse(409, "Not enough currency")]
    public async Task<ActionResult<CurrencyResponse>> ModifyCurrency(
      [SwaggerParameter("Character identifier", Required = true)] Guid characterId,
      [FromBody] [SwaggerRequestBody("Currency modification details", Required = true)] ModifyCurrencyRequest request) {
      if (request.Gold == null &&
          request.Silver == null &&
          request.Copper == null &&
          request.Electrum == null &&
          request.Platinum == null) {
        return BadRequest(new {
          error = "NO_CURRENCY_PROVIDED",
          message = "At least one currency must be specified."
        });
      }

      try {
        var currencyChanges = mapper.Map<Dictionary<CurrencyType, int>>(request);
        var character = await currencyService.ModifyCurrenciesAsync(characterId, currencyChanges);
        var currencyResponse = mapper.Map<CurrencyResponse>(character.Wealth);
        return Ok(currencyResponse);
      } catch (CharacterNotFoundException) {
        return NotFound(new {
          error = "CHARACTER_NOT_FOUND",
          message = "Character not found."
        });
      } catch (CurrencyNotInitializedException) {
        return Conflict(new {
          error = "CURRENCY_NOT_INITIALIZED",
          message = "Currency has not been initialized for this character."
        });
      } catch (NotEnoughCurrencyException) {
        return Conflict(new {
          error = "NOT_ENOUGH_CURRENCY",
          message = "Not enough currency to perform the operation."
        });
      } catch (OverflowException) {
        return StatusCode(500,
                          new {
                            error = "INTERNAL_SERVER_ERROR",
                            message = "An unexpected error occurred during currency exchange."
                          });
      }
    }

    /// <summary>
    /// Exchanges a specified amount of one type of currency ('from') for another ('to')
    /// using standard D&D 5e conversion rates (flooring any fractional results).
    /// For example, exchanging 1 Gold ('from') with 'amount' 1 for Silver ('to')
    /// will deduct 1 Gold and add 10 Silver (1 GP = 10 SP).
    /// Exchanging 11 Silver ('from') with 'amount' 1 for Gold ('to')
    /// will deduct 10 Silver and add 1 Gold.
    /// </summary>
    /// <param name="characterId">The unique identifier of the character.</param>
    /// <param name="request">Details for the exchange, specifying the currency type to pay with ('From'),
    /// the currency type to receive ('To'), and the exact amount of the 'From' currency to spend ('Amount').</param>
    /// <returns>The updated currency amounts after the exchange.</returns>
    [HttpPatch("exchange")]
    [SwaggerOperation(Summary = "Exchange Character Currency (Spend 'From' Amount)",
                       Description =
                         "Converts a specified `Amount` of the `From` currency into the `To` currency, based on standard D&D 5e" +
                         " conversion rates. Any fractional results for the `To` currency are not converted.\n\n" +
                         "Example 1: Spend 2 Gold for Silver\n" +
                         "Request Body: `{ \"from\": \"gold\", \"to\": \"silver\", \"amount\": 2 }`\n" +
                         "Result: Deduct 2 Gold, Add 20 Silver.\n\n" +
                         "Example 2: Spend 15 Silver for Gold\n" +
                         "Request Body: `{ \"from\": \"silver\", \"to\": \"gold\", \"amount\": 15 }`\n" +
                         "Result: Deduct 10 Silver, Add 1 Gold.")]
    [SwaggerResponse(200, "Currency Exchanged", typeof(CurrencyResponse))]
    [SwaggerResponse(400, "Invalid Request / Invalid Argument (e.g., negative amount)", typeof(object))]
    [SwaggerResponse(400, "Invalid Exchange (e.g., same currency type)", typeof(object))]
    [SwaggerResponse(404, "Character Not Found", typeof(object))]
    [SwaggerResponse(400, "Not Enough 'From' Currency", typeof(object))]
    public async Task<ActionResult<CurrencyResponse>> ExchangeCurrency(
      [SwaggerParameter("Character identifier", Required = true)] Guid characterId,
      [FromBody]
      [SwaggerRequestBody("Currency exchange details (specify amount of 'From' currency to spend)", Required = true)]
      ExchangeCurrencyRequest request) {
      try {
        var character =
          await currencyService.ExchangeCurrencyAsync(characterId, request.From, request.To, request.Amount);
        var currencyResponse = mapper.Map<CurrencyResponse>(character.Wealth);
        return Ok(currencyResponse);
      } catch (CharacterNotFoundException ex) {
        return NotFound(new {
          error = "CHARACTER_NOT_FOUND",
          message = ex.Message
        });
      } catch (CurrencyNotInitializedException ex) {
        return Conflict(new {
          error = "CURRENCY_NOT_INITIALIZED",
          message = ex.Message
        });
      } catch (NotEnoughCurrencyException ex) {
        return Conflict(new {
          error = "NOT_ENOUGH_CURRENCY",
          message = ex.Message
        });
      } catch (InvalidCurrencyExchangeException ex) {
        return BadRequest(new {
          error = "INVALID_CURRENCY_EXCHANGE",
          message = ex.Message
        });
      } catch (OverflowException) {
        return StatusCode(500,
                          new {
                            error = "INTERNAL_SERVER_ERROR",
                            message = "An unexpected error occurred during currency exchange."
                          });
      }
    }
  }
}
