using Microsoft.AspNetCore.Mvc;
using RPGCharacterService.Dtos.Dice;
using RPGCharacterService.Models;
using RPGCharacterService.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace RPGCharacterService.Controllers {
  /// <summary>
  /// Controller responsible for handling dice rolling operations.
  /// Provides functionality to roll various types of dice commonly used in tabletop RPGs.
  /// </summary>
  [ApiController]
  [ApiVersion("1.0")]
  [Route("api/v{version:apiVersion}/dice")]
  public class DiceController(IDiceService diceService) : ControllerBase {
    /// <summary>
    /// Rolls one or more dice of a specified type.
    /// Supports standard RPG dice types: d4, d6, d8, d10, d12, and d20.
    /// </summary>
    /// <param name="request">The dice rolling request containing the number of sides and count of dice to roll.</param>
    /// <returns>A response containing the results of each die roll.</returns>
    /// <remarks>
    /// The dice sides must be one of the following values: 4, 6, 8, 10, 12, or 20.
    /// Each die roll will return a random integer between 1 and the number of sides (inclusive).
    /// </remarks>
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
