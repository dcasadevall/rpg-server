using Microsoft.AspNetCore.Mvc;
using RPGCharacterService.Dtos.Stats.Requests;
using RPGCharacterService.Dtos.Stats.Responses;
using RPGCharacterService.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace RPGCharacterService.Controllers {
  /// <summary>
  /// Controller responsible for managing character statistics such as hit points.
  /// In the future, this will also handle experience points.
  /// </summary>
  [ApiController]
  [ApiVersion("1.0")]
  [Route("api/v{version:apiVersion}/characters/{characterId:guid}/stats")]
  public class StatsController(IStatsService statsService) : ControllerBase {
    /// <summary>
    /// Modifies a character's hit points by a specified amount.
    /// The hit points will be clamped between 0 and the character's maximum hit points.
    /// </summary>
    /// <param name="characterId">The unique identifier of the character.</param>
    /// <param name="request">The hit points modification details containing the delta value.</param>
    /// <returns>The updated character with modified hit points.</returns>
    /// <remarks>
    /// A positive delta increases hit points (healing), while a negative delta decreases hit points (damage).
    /// The final hit points value will never exceed the character's maximum hit points or go below 0.
    /// </remarks>
    [HttpPatch("hitpoints")]
    [SwaggerOperation(Summary = "Update Character Hit Points",
                       Description = "Adds or subtracts the specified delta from the character's current hit points")]
    [SwaggerResponse(200, "Hit Points Updated", typeof(HitPointUpdateResponse))]
    [SwaggerResponse(400, "Invalid ID or Delta")]
    [SwaggerResponse(404, "Character Not Found")]
    public async Task<ActionResult<HitPointUpdateResponse>> ModifyHitPoints(
      [SwaggerParameter("Character identifier", Required = true)] Guid characterId,
      [FromBody] [SwaggerRequestBody("Hit points modification details", Required = true)] HitPointUpdateRequest request) {
      try {
        if (!ModelState.IsValid) {
          return BadRequest(new {errors = ModelState});
        }

        var character = await statsService.ModifyHitPointsAsync(characterId, request.Delta);
        return Ok(new HitPointUpdateResponse {HitPoints = character.HitPoints});
      } catch (KeyNotFoundException) {
        return NotFound(new {error = "CHARACTER_NOT_FOUND", message = "Character not found."});
      } catch (Exception ex) {
        return BadRequest(new {error = "INVALID_DELTA", message = ex.Message});
      }
    }
  }
}
