using Microsoft.AspNetCore.Mvc;
using RPGCharacterService.Dtos.Currency.Requests;
using RPGCharacterService.Dtos.Currency.Responses;
using RPGCharacterService.Services;
using RPGCharacterService.Mappers;
using Swashbuckle.AspNetCore.Annotations;

namespace RPGCharacterService.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:ApiVersion}/characters/{characterId:guid}/currency")]
    public class CurrencyController(ICurrencyService currencyService, ICharacterService characterService)
        : ControllerBase
    {
        [HttpPost("init")]
        [SwaggerOperation(Summary = "Initialize a Character's Currency", 
                             Description = "Randomly sets the character's starting currency by rolling dice")]
        [SwaggerResponse(200, "Currency Initialized", typeof(CurrencyResponse))]
        [SwaggerResponse(400, "Invalid ID Format")]
        [SwaggerResponse(404, "Character Not Found")]
        [SwaggerResponse(409, "Currency Already Initialized")]
        public async Task<ActionResult<CurrencyResponse>> InitializeCurrency(
            [SwaggerParameter("Character identifier", Required = true)] Guid characterId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    errors = ModelState
                });
            }

            try
            {
                var character = await currencyService.GenerateInitialCurrencyAsync(characterId);
                var currencyResponse = CurrencyMapper.ToCurrencyResponse(character.Wealth);
                return Ok(currencyResponse);
            } catch (KeyNotFoundException)
            {
                return NotFound(new
                {
                    error = "CHARACTER_NOT_FOUND",
                    message = "Character not found."
                });
            } catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    error = "CURRENCY_ALREADY_INITIALIZED",
                    message = ex.Message
                });
            }
        }

        [HttpPatch]
        [SwaggerOperation(Summary = "Modify Character Currency", 
                             Description = "Adds or subtracts the specified values from the character's current currency")]
        [SwaggerResponse(200, "Currency Modified", typeof(CurrencyResponse))]
        [SwaggerResponse(400, "Invalid Request")]
        [SwaggerResponse(404, "Character Not Found")]
        [SwaggerResponse(409, "Currency Not Initialized")]
        public async Task<ActionResult<CurrencyResponse>> ModifyCurrency(
            [SwaggerParameter("Character identifier", Required = true)] Guid characterId, 
            [FromBody][SwaggerRequestBody("Currency modification details", Required = true)] ModifyCurrencyRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        errors = ModelState
                    });
                }
                
                if (request.Gold == null && request.Silver == null &&
                    request.Copper == null && request.Electrum == null && request.Platinum == null)
                {
                    return BadRequest(new
                    {
                        error = "NO_CURRENCY_PROVIDED",
                        message = "At least one currency must be specified."
                    });
                }

                var currencyChanges = CurrencyMapper.ToDictionary(request);
                var character = await currencyService.ModifyCurrenciesAsync(characterId, currencyChanges);
                var currencyResponse = CurrencyMapper.ToCurrencyResponse(character.Wealth);
                return Ok(currencyResponse);
            } catch (KeyNotFoundException)
            {
                return NotFound(new
                {
                    error = "CHARACTER_NOT_FOUND",
                    message = "Character not found."
                });
            } catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = "INVALID_CURRENCY",
                    message = ex.Message
                });
            }
        }

        [HttpPatch("exchange")]
        [SwaggerOperation(Summary = "Exchange Character Currency", 
                         Description = "Converts the specified amount of currency from one type to another using standard D&D 5e conversions")]
        [SwaggerResponse(200, "Currency Exchanged", typeof(CurrencyResponse))]
        [SwaggerResponse(400, "Invalid Request")]
        [SwaggerResponse(400, "Invalid Exchange")]
        [SwaggerResponse(404, "Character Not Found")]
        [SwaggerResponse(409, "Currency Not Initialized")]
        [SwaggerResponse(409, "Not Enough Currency")]
        public async Task<ActionResult<CurrencyResponse>> ExchangeCurrency(
            [SwaggerParameter("Character identifier", Required = true)] Guid characterId,
            [FromBody][SwaggerRequestBody("Currency exchange details", Required = true)] ExchangeCurrencyRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        errors = ModelState
                    });
                }

                // TODO: Handle exception and dont return here
                if (request.From == request.To)
                {
                    return BadRequest(new
                    {
                        error = "INVALID_CURRENCY_CONVERSION",
                        message = "Cannot convert between the same currency."
                    });
                }

                var character = await currencyService.ExchangeCurrencyAsync(characterId, request.From, request.To, request.Amount);
                var currencyResponse = CurrencyMapper.ToCurrencyResponse(character.Wealth);
                return Ok(currencyResponse);
            } catch (KeyNotFoundException)
            {
                return NotFound(new
                {
                    error = "CHARACTER_NOT_FOUND",
                    message = "Character not found."
                });
            } catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = "INVALID_CURRENCY_VALUE",
                    message = ex.Message
                });
            }
        }
    }
}
