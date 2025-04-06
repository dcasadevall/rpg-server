using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using RPGCharacterService.Services;

namespace RPGCharacterService.Controllers
{
    [ApiController]
    [Route("api/v1/characters/{characterId:guid}/stats")]
    public class StatsController(IStatsService statsService) : ControllerBase
    {
        [HttpPatch("hitpoints")]
        public ActionResult<object> ModifyHitPoints(Guid characterId, [FromBody] HitPointUpdateRequest request)
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
