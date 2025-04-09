using RPGCharacterService.Exceptions.Character;
using RPGCharacterService.Entities.Characters;

namespace RPGCharacterService.Persistence.Characters {
  /// <summary>
  /// Defines the interface for character management.
  /// </summary>
  public interface ICharacterRepository {
    /// <summary>
    /// Retrieves all characters from the repository.
    /// </summary>
    Task<IEnumerable<Character>> GetAllAsync();

    /// <summary>
    /// Retrieves a character by its unique identifier.
    /// </summary>
    Task<Character?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves a character by their name.
    /// </summary>
    Task<Character?> GetByNameAsync(string name);

    /// <summary>
    /// Adds a new character to the repository.
    /// </summary>
    Task AddAsync(Character character);

    /// <summary>
    /// Updates an existing character in the repository.
    /// </summary>
    Task UpdateAsync(Character character);

    /// <summary>
    /// Deletes a character from the repository.
    /// </summary>
    Task DeleteAsync(Guid id);
  }

  public static class CharacterRepositoryExtensions {
    /// <summary>
    /// Retrieves a character by its unique identifier, throwing an exception if it is not found.
    /// </summary>
    public static async Task<Character> GetByIdOrThrowAsync(this ICharacterRepository repository, Guid id) {
      var character = await repository.GetByIdAsync(id);
      if (character == null) {
        throw new CharacterNotFoundException(id);
      }

      return character;
    }
  }
}
