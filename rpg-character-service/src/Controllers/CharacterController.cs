using Microsoft.AspNetCore.Mvc;
using RPGCharacterService.Dtos.Character.Requests;
using RPGCharacterService.Dtos.Character.Responses;
using RPGCharacterService.Mappers;
using RPGCharacterService.Models.Characters;
using RPGCharacterService.Models.Items;
using RPGCharacterService.Rules;
using RPGCharacterService.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace RPGCharacterService.Controllers {
  [ApiController]
  [ApiVersion("1.0")]
  [Route("api/v{version:apiVersion}/characters")]
  public class CharacterController(ICharacterService characterService,
                                   ICharacterRules characterRules,
                                   IEquipmentRules equipmentRules) : ControllerBase {
    [HttpGet]
    [SwaggerOperation(Summary = "Retrieve All Characters", Description = "Gets a list of all characters")]
    [SwaggerResponse(200, "Successful Response", typeof(List<CharacterResponse>))]
    public async Task<ActionResult<List<CharacterResponse>>> GetAllCharacters() {
      var characters = await characterService.GetAllCharactersAsync();
      return Ok(characters
                .Select(x => {
                  var derivedProps = CalculateDerivedProperties(x, characterRules, equipmentRules);
                  return CharacterMapper.ToResponse(x, derivedProps);
                })
                .ToList());
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Retrieve Character Information", Description = "Gets a specific character by their ID")]
    [SwaggerResponse(200, "Successful Response", typeof(CharacterResponse))]
    [SwaggerResponse(400, "Invalid ID Format")]
    [SwaggerResponse(404, "Character Not Found")]
    public async Task<ActionResult<CharacterResponse>> GetCharacterById(
      [SwaggerParameter("Character identifier", Required = true)] Guid id) {
      try {
        var character = await characterService.GetCharacterAsync(id);
        var derivedProps = CalculateDerivedProperties(character, characterRules, equipmentRules);
        var characterResponse = CharacterMapper.ToResponse(character, derivedProps);
        return Ok(characterResponse);
      } catch (KeyNotFoundException) {
        return NotFound(new {error = "CHARACTER_NOT_FOUND", message = "Character not found."});
      }
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a New Character",
                       Description = "Creates a new character with the provided attributes")]
    [SwaggerResponse(201, "Character created successfully")]
    [SwaggerResponse(400, "Bad Request - Invalid input format")]
    [SwaggerResponse(409, "Conflict - Name already taken or other conflict")]
    public async Task<ActionResult<CharacterResponse>> CreateCharacter(
      [FromBody] [SwaggerRequestBody("Character creation details", Required = true)]
      CreateCharacterRequest characterRequest) {
      if (!ModelState.IsValid) {
        // TODO: Do not dump ModelState directly
        return BadRequest(new {errors = ModelState});
      }

      try {
        var newCharacter = await characterService.CreateCharacterAsync(characterRequest);
        return CreatedAtAction(nameof(GetCharacterById), new {id = newCharacter.Id}, newCharacter);
      } catch (Exception ex) {
        return BadRequest(new {error = ex.Message});
      }
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Delete a Character", Description = "Deletes the character with the specified ID")]
    [SwaggerResponse(204, "No Content - Character successfully deleted")]
    [SwaggerResponse(400, "Invalid ID Format")]
    [SwaggerResponse(404, "Character Not Found")]
    public async Task<ActionResult> DeleteCharacter([SwaggerParameter("Character identifier", Required = true)] Guid id) {
      try {
        await characterService.DeleteCharacterAsync(id);
        return NoContent();
      } catch (KeyNotFoundException) {
        return NotFound(new {error = "CHARACTER_NOT_FOUND", message = "Character not found."});
      }
    }

    private static CharacterDerivedProperties CalculateDerivedProperties(Character character,
                                                                         ICharacterRules characterRules,
                                                                         IEquipmentRules equipmentRules) {
      var derivedProperties = new CharacterDerivedProperties {
        MaxHitPoints = characterRules.CalculateMaxHitPoints(character),
        ArmorClass = equipmentRules.CalculateArmorClass(character),
        ProficiencyBonus = characterRules.CalculateProficiencyBonus(character),
        AbilityModifiers = characterRules.CalculateAbilityModifiers(character)
      };

      return derivedProperties;
    }
  }
}
