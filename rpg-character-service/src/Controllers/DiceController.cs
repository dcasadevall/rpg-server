using Microsoft.AspNetCore.Mvc;
using RPGCharacterService.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace RPGCharacterService.Controllers
{
    [ApiController]
    [Route("api/v1/dice")]
    public class DiceController(IDiceService diceService) : ControllerBase
    {
        private static readonly HashSet<int> validSides = [4, 6, 8, 10, 12, 20];
        
        [HttpGet("roll")]
        [SwaggerOperation(Summary = "Roll a Dice", 
                         Description = "Rolls one or more dice with the specified number of sides. " +
                                       "Returns random integer results between 1 and sides (inclusive) for each die rolled.")]
        [SwaggerResponse(200, "Successful dice roll", typeof(object))]
        [SwaggerResponse(400, "Invalid sides or count format")]
        [SwaggerResponse(422, "Invalid dice type - not a valid dice type")]
        public ActionResult<object> RollDice([FromQuery][SwaggerParameter("Number of sides on the dice", Required = true)] int sides, 
                                             [FromQuery][SwaggerParameter("Number of dice to roll (default 1)")] int count = 1)
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
                
                // Return array of results as per the API.md specification
                return Ok(new { 
                    results = rolls
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
