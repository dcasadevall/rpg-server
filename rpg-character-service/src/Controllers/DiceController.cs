using Microsoft.AspNetCore.Mvc;
using RPGCharacterService.Dtos.Dice;
using RPGCharacterService.Models;
using RPGCharacterService.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace RPGCharacterService.Controllers {
  [ApiController]
  [ApiVersion("1.0")]
  [Route("api/v{version:apiVersion}/dice")]
  public class DiceController(IDiceService diceService) : ControllerBase {
    [HttpGet("roll")]
    [SwaggerOperation(Summary = "Roll a Dice",
                       Description = "Rolls one or more dice. Allowed sides: 4, 6, 8, 10, 12, 20." +
                                     "Returns random integer results between 1 and sides (inclusive) for each die rolled.")]
    [SwaggerResponse(200, "Successful dice roll", typeof(RollDiceResponse))]
    [SwaggerResponse(400, "Invalid sides or count format")]
    [SwaggerResponse(422, "Invalid dice type - not a valid dice type")]
    public ActionResult<RollDiceResponse> RollDice([FromQuery] RollDiceRequest request) {
      try {
        // We could use DiceSides as the parameter type, and swagger would handle the enum conversion
        // plus show a dropdown in the UI, but if we do that, we would not be able to properly
        // send an INVALID_SIDES message without a custom model binder, which overcomplicates things.
        if (!Enum.IsDefined(typeof(DiceSides), request.Sides)) {
          return UnprocessableEntity(new {
            error = "INVALID_SIDES",
            message = "Invalid number of sides. Must be one of: 4, 6, 8, 10, 12, 20."
          });
        }

        var rolls = diceService.Roll(request.Sides, request.Count);
        return Ok(new RollDiceResponse {
          Results = rolls
        });
      } catch (FormatException) {
        return BadRequest(new {
          error = "INVALID_PARAMETER_FORMAT",
          message = "Provided sides or count is not a valid integer."
        });
      } catch (ArgumentException ex) {
        return BadRequest(new {
          error = "INVALID_PARAMETERS",
          message = ex.Message
        });
      }
    }
  }
}
