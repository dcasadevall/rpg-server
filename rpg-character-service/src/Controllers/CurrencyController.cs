using Microsoft.AspNetCore.Mvc;
using RPGCharacterService.Dtos.Currency.Requests;
using RPGCharacterService.Dtos.Currency.Responses;
using RPGCharacterService.Services;
using RPGCharacterService.Mappers;

namespace RPGCharacterService.Controllers
{
    [ApiController]
    [Route("api/v1/characters/{characterId:guid}/currency")]
    public class CurrencyController(ICurrencyService currencyService, ICharacterService characterService)
        : ControllerBase
    {
        [HttpPost("init")]
        public ActionResult<CurrencyResponse> InitializeCurrency(Guid characterId)
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
                var character = currencyService.GenerateInitialCurrency(characterId);
                return Ok(new
                {
                    currencies = CurrencyMapper.ToResponse(character.Currencies)
                });
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
        public ActionResult<CurrencyResponse> ModifyCurrency(Guid characterId, [FromBody] ModifyCurrencyRequest request)
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
                
                if (request.Gold == null && request.Silver == null && request.Bronze == null &&
                    request.Copper == null && request.Electrum == null && request.Platinum == null)
                {
                    return BadRequest(new
                    {
                        error = "NO_CURRENCY_PROVIDED",
                        message = "At least one currency must be specified."
                    });
                }

                var currencies = CurrencyMapper.ToCurrencyDictionary(request);
                
                // TODO: Try/ catch. Services should throw concrete exceptions.
                // Controllers should validate basic input (null and required fields)
                var character = currencyService.ModifyCurrencies(characterId, currencies);
                return Ok(new
                {
                    currencies = CurrencyMapper.ToResponse(character.Currencies)
                });
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
        public ActionResult<CurrencyResponse> ExchangeCurrency(Guid characterId,
                                                              [FromBody] ExchangeCurrencyRequest request)
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

                if (request.From == request.To)
                {
                    return BadRequest(new
                    {
                        error = "INVALID_CURRENCY_CONVERSION",
                        message = "Cannot convert between the same currency."
                    });
                }

                var character = currencyService.ExchangeCurrency(characterId, request.From, request.To, request.Amount);
                return Ok(new
                {
                    currencies = CurrencyMapper.ToResponse(character.Currencies)
                });
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
