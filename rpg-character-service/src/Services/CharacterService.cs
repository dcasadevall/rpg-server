using RPGCharacterService.Dtos.Character;
using RPGCharacterService.Exceptions.Character;
using RPGCharacterService.Entities;
using RPGCharacterService.Entities.Characters;
using RPGCharacterService.Persistence;

namespace RPGCharacterService.Services {
  public interface ICharacterService {
    /// <summary>
    /// Retrieves all characters from the system.
    /// </summary>
    /// <returns>A collection of all characters.</returns>
    Task<IEnumerable<Character>> GetAllCharactersAsync();

    /// <summary>
    /// Retrieves a specific character by their unique identifier.
    /// </summary>
    /// <param name="characterId">The unique identifier of the character to retrieve.</param>
    /// <returns>The character with the specified identifier.</returns>
    /// <exception cref="CharacterNotFoundException">Thrown when no character exists with the specified ID.</exception>
    Task<Character> GetCharacterAsync(Guid characterId);

    /// <summary>
    /// Creates a new character with the specified details.
    /// </summary>
    /// <param name="request">The request containing the character's details.</param>
    /// <returns>The newly created character.</returns>
    /// <exception cref="InappropriateNameException">Thrown when the character name is inappropriate.</exception>
    /// <exception cref="CharacterAlreadyExistsException">Thrown when a character with the same name already exists.</exception>
    Task<Character> CreateCharacterAsync(CreateCharacterRequest request);

    /// <summary>
    /// Deletes a character with the specified identifier.
    /// </summary>
    /// <param name="characterId">The unique identifier of the character to delete.</param>
    /// <exception cref="CharacterNotFoundException">Thrown when no character exists with the specified ID.</exception>
    Task DeleteCharacterAsync(Guid characterId);
  }

  /// <summary>
  /// Provides functionality for managing characters.
  /// This service handles character creation, retrieval, and deletion, including validation and initialization.
  /// </summary>
  public class CharacterService(ICharacterRepository repository,
                                IDiceService diceService,
                                ICharacterValidator characterValidator) : ICharacterService {
    /// <summary>
    /// Retrieves all characters from the system.
    /// </summary>
    /// <returns>A collection of all characters.</returns>
    public async Task<IEnumerable<Character>> GetAllCharactersAsync() {
      return await repository.GetAllAsync();
    }

    /// <summary>
    /// Retrieves a specific character by their unique identifier.
    /// </summary>
    /// <param name="characterId">The unique identifier of the character to retrieve.</param>
    /// <returns>The character with the specified identifier.</returns>
    /// <exception cref="CharacterNotFoundException">Thrown when no character exists with the specified ID.</exception>
    public async Task<Character> GetCharacterAsync(Guid characterId) {
      return await repository.GetByIdOrThrowAsync(characterId);
    }

    /// <summary>
    /// Creates a new character with the specified details.
    /// </summary>
    /// <param name="request">The request containing the character's details.</param>
    /// <returns>The newly created character.</returns>
    /// <exception cref="CharacterAlreadyExistsException">Thrown when a character with the same name already exists.</exception>
    /// <exception cref="InvalidRaceException">Thrown when the race is invalid.</exception>
    /// <exception cref="InappropriateNameException">Thrown when the character name is inappropriate.</exception>
    public async Task<Character> CreateCharacterAsync(CreateCharacterRequest request) {
      // Additional validation that is specific to the character domain
      characterValidator.ValidateCharacterDetails(request.Name, request.Race, request.Subrace, request.Class);

      // NOTE: For the scope of this prompt, we are not using transactions.
      // In a production environment these kind of checks before creation need to be atomic,
      // and would require a transaction to be created and used for both GetByNameAsync and AddAsync.
      var existingCharacter = await repository.GetByNameAsync(request.Name);
      if (existingCharacter != null) {
        throw new CharacterAlreadyExistsException(request.Name);
      }

      // First, roll for all ability scores
      var abilityScores = new Dictionary<AbilityScore, int>();
      foreach (var stat in Enum.GetValues<AbilityScore>()) {
        // Roll 4d6 and take the highest 3
        abilityScores[stat] = diceService
                              .Roll(DiceSides.Six, 4)
                              .OrderByDescending(x => x)
                              .Take(3)
                              .Sum();
      }

      // Create character and set initial HP based on Max HP
      var character = new Character {
        Name = request.Name,
        Race = request.Race,
        Subrace = request?.Subrace ?? "",
        Class = request.Class,
        Level = 1,
        AbilityScores = abilityScores
      };
      character.HitPoints = character.CalculateMaxHitPoints();

      await repository.AddAsync(character);
      return character;
    }

    /// <summary>
    /// Deletes a character with the specified identifier.
    /// </summary>
    /// <param name="characterId">The unique identifier of the character to delete.</param>
    /// <exception cref="CharacterNotFoundException">Thrown when no character exists with the specified ID.</exception>
    public Task DeleteCharacterAsync(Guid characterId) {
      return repository.DeleteAsync(characterId);
    }
  }
}
