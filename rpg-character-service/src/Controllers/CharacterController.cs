using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RPGCharacterService.Dtos.Character;
using RPGCharacterService.Exceptions.Character;
using RPGCharacterService.Mapping;
using RPGCharacterService.Persistence;
using RPGCharacterService.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace RPGCharacterService.Controllers {
  /// <summary>
  /// Controller responsible for handling character-related HTTP requests.
  /// Provides endpoints for creating, retrieving, updating, and deleting characters,
  /// as well as managing their equipment and wealth.
  /// </summary>
  [ApiController]
  [ApiVersion("1.0")]
  [Route("api/v{version:apiVersion}/characters")]
  public class CharacterController(ICharacterService characterService, LoggedMapper mapper) : ControllerBase {
    /// <summary>
    /// Retrieves a list of all characters.
    /// </summary>
    /// <returns>A list of character responses.</returns>
    /// <response code="200">Returns the list of characters.</response>
    [HttpGet]
    [SwaggerOperation(Summary = "Retrieve All Characters", Description = "Gets a list of all characters")]
    [SwaggerResponse(200, "Successful Response", typeof(List<CharacterResponse>))]
    public async Task<ActionResult<List<CharacterResponse>>> GetAllCharacters() {
      var characters = await characterService.GetAllCharactersAsync();
      return Ok(characters
                .Select(c => mapper.Map<CharacterResponse>(c))
                .ToList());
    }

    /// <summary>
    /// Retrieves a character by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the character.</param>
    /// <returns>The character's information if found.</returns>
    /// <response code="200">Returns the requested character.</response>
    /// <response code="400">If the ID format is invalid.</response>
    /// <response code="404">If the character with the specified ID is not found.</response>
    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Retrieve Character Information", Description = "Gets a specific character by their ID")]
    [SwaggerResponse(200, "Successful Response", typeof(CharacterResponse))]
    [SwaggerResponse(400, "Invalid ID Format")]
    [SwaggerResponse(404, "Character Not Found")]
    public async Task<ActionResult<CharacterResponse>> GetCharacterById(
      [SwaggerParameter("Character identifier", Required = true)] Guid id) {
      try {
        var character = await characterService.GetCharacterAsync(id);
        return Ok(mapper.Map<CharacterResponse>(character));
      } catch (CharacterNotFoundException) {
        return NotFound(new {
          error = "CHARACTER_NOT_FOUND",
          message = "Character not found."
        });
      }
    }

    /// <summary>
    /// Creates a new character with the specified details.
    /// </summary>
    /// <param name="request">The character creation request containing the character's details.</param>
    /// <returns>The newly created character's information.</returns>
    /// <response code="201">Returns the newly created character.</response>
    /// <response code="400">If the request is invalid or the character name is inappropriate.</response>
    /// <response code="409">If a character with the same name already exists.</response>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a New Character",
                       Description = "Creates a new character with the provided attributes")]
    [SwaggerResponse(201, "Character created successfully")]
    [SwaggerResponse(400, "Bad Request - Invalid input format")]
    [SwaggerResponse(409, "Conflict - Name already taken or other conflict")]
    public async Task<ActionResult<CharacterResponse>> CreateCharacter(
      [FromBody] [SwaggerRequestBody("Character creation details", Required = true)]
      CreateCharacterRequest request) {
      try {
        var newCharacter = await characterService.CreateCharacterAsync(request);
        var characterResponse = mapper.Map<CharacterResponse>(newCharacter);
        return CreatedAtAction(nameof(GetCharacterById), new {id = newCharacter.Id}, characterResponse);
      } catch (InappropriateNameException) {
        return BadRequest(new {error = "NAME_INAPPROPRIATE", message = "The provided name is inappropriate and cannot be used."});
      } catch (CharacterAlreadyExistsException) {
        return Conflict(new {error = "NAME_ALREADY_TAKEN", message = "A character with this name already exists."});
      }
    }

    /// <summary>
    /// Deletes a character by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the character to delete.</param>
    /// <returns>No content if the deletion was successful.</returns>
    /// <response code="204">If the character was successfully deleted.</response>
    /// <response code="400">If the ID format is invalid.</response>
    /// <response code="404">If the character with the specified ID is not found.</response>
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Delete a Character", Description = "Deletes the character with the specified ID")]
    [SwaggerResponse(204, "No Content - Character successfully deleted")]
    [SwaggerResponse(400, "Invalid ID Format")]
    [SwaggerResponse(404, "Character Not Found")]
    public async Task<ActionResult> DeleteCharacter([SwaggerParameter("Character identifier", Required = true)] Guid id) {
      try {
        await characterService.DeleteCharacterAsync(id);
        return NoContent();
      } catch (CharacterNotFoundException) {
        return NotFound(new {error = "CHARACTER_NOT_FOUND", message = "Character not found."});
      }
    }
  }
}
