using Microsoft.AspNetCore.Mvc;
using RPGCharacterService.Dtos.Character.Requests;
using RPGCharacterService.Dtos.Character.Responses;
using RPGCharacterService.Services;

namespace RPGCharacterService.Controllers
{
    [ApiController]
    [Route("api/v1/characters")]
    public class CharacterController(ICharacterService characterService) : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<CharacterResponse>> GetAllCharacters()
        {
            return Ok(characterService.GetAllCharacters());
        }
        
        [HttpGet("{id:guid}")]
        public ActionResult<CharacterResponse> GetCharacterById(Guid id)
        {
            try
            {
                return Ok(characterService.GetCharacterById(id));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = "CHARACTER_NOT_FOUND", message = "Character not found." });
            }
        }
        
        [HttpPost]
        public ActionResult<CharacterResponse> CreateCharacter([FromBody] CreateCharacterRequest characterRequest)
        {
            if (!ModelState.IsValid)
            {
                // TODO: Do not dump ModelState directly
                return BadRequest(new { errors = ModelState });
            }
            
            try
            {
                var newCharacter = characterService.CreateCharacter(characterRequest);
                return CreatedAtAction(nameof(GetCharacterById), new { id = newCharacter.Id }, newCharacter);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        
        [HttpDelete("{id:guid}")]
        public ActionResult DeleteCharacter(Guid id)
        {
            try
            {
                characterService.DeleteCharacter(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = "CHARACTER_NOT_FOUND", message = "Character not found." });
            }
        }
    }
} 
