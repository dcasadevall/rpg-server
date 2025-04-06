using Microsoft.AspNetCore.Mvc;
using RPGCharacterService.Services;

namespace RPGCharacterService.Controllers
{
    [ApiController]
    [Route("api/v1/dice")]
    public class DiceController(IDiceService diceService) : ControllerBase
    {
        private static readonly HashSet<int> validSides = [4, 6, 8, 10, 12, 20];
        
        [HttpGet("roll")]
        public ActionResult<object> RollDice([FromQuery] int sides, [FromQuery] int count = 1)
        {
            try
            {
                if (!validSides.Contains(sides))
                {
                    return UnprocessableEntity(new 
                    { 
                        error = "INVALID_SIDES",
                        message = "Invalid number of sides. Must be one of: 4, 6, 8, 10, 12, 20."
                    });
                }
                
                if (count is <= 0 or > 100)
                {
                    return BadRequest(new 
                    { 
                        error = "INVALID_COUNT",
                        message = "Count must be between 1 and 100."
                    });
                }
                
                var rolls = diceService.Roll(sides, count);
                
                // If count is 1, return the simpler result format
                if (count == 1)
                {
                    return Ok(new { result = rolls[0] });
                }
                
                // Otherwise return array of results and total
                return Ok(new { 
                    results = rolls,
                    total = rolls.Sum()
                });
            }
            catch (FormatException)
            {
                return BadRequest(new 
                { 
                    error = "INVALID_PARAMETER_FORMAT",
                    message = "Provided sides or count is not a valid integer."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new 
                { 
                    error = "INVALID_PARAMETERS",
                    message = ex.Message
                });
            }
        }
    }
} 
