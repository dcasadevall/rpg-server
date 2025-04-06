using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using RPGCharacterService.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace RPGCharacterService.Controllers
{
    [ApiController]
    [Route("api/v1/characters/{characterId:guid}/stats")]
    public class StatsController(IStatsService statsService) : ControllerBase
    {
        [HttpPatch("hitpoints")]
        [SwaggerOperation(Summary = "Update Character Hit Points", 
                         Description = "Adds or subtracts the specified delta from the character's current hit points")]
        [SwaggerResponse(200, "Hit Points Updated", typeof(object))]
        [SwaggerResponse(400, "Invalid ID or Delta")]
        [SwaggerResponse(404, "Character Not Found")]
        public ActionResult<object> ModifyHitPoints(
            [SwaggerParameter("Character identifier", Required = true)] Guid characterId, 
            [FromBody][SwaggerRequestBody("Hit points modification details", Required = true)] HitPointUpdateRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { errors = ModelState });
                }
                
                var character = statsService.ModifyHitPoints(characterId, request.Delta);
                return Ok(new { hitPoints = character.HitPoints });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = "CHARACTER_NOT_FOUND", message = "Character not found." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "INVALID_DELTA", message = ex.Message });
            }
        }
    }
    
    public class HitPointUpdateRequest
    {
        [Required]
        public int Delta { get; set; }
    }
} 
