using RPGCharacterService.Entities.Characters;

namespace RPGCharacterService.Persistence.Characters {
  /// <summary>
  /// In-memory implementation of ICharacterRepository for local testing and development.
  /// This implementation stores characters in a dictionary and is not suitable for production use
  /// as it does not persist data between application restarts and is not thread-safe.
  /// </summary>
  public class InMemoryCharacterRepository : ICharacterRepository {
    private readonly Dictionary<Guid, Character> characters = new();

    /// <summary>
    /// Retrieves all characters from the in-memory store.
    /// </summary>
    /// <returns>An enumerable of all stored characters.</returns>
    public Task<IEnumerable<Character>> GetAllAsync() {
      return Task.FromResult(characters.Values.AsEnumerable());
    }

    /// <summary>
    /// Retrieves a character by its unique identifier from the in-memory store.
    /// </summary>
    /// <param name="id">The unique identifier of the character.</param>
    /// <returns>The character if found, null otherwise.</returns>
    public Task<Character?> GetByIdAsync(Guid id) {
      return Task.FromResult(characters.GetValueOrDefault(id));
    }

    /// <summary>
    /// Retrieves a character by its name from the in-memory store.
    /// The search is case-insensitive.
    /// </summary>
    /// <param name="name">The name of the character to find.</param>
    /// <returns>The character if found, null otherwise.</returns>
    public Task<Character?> GetByNameAsync(string name) {
      return Task.FromResult(characters.Values.FirstOrDefault(c => c.Name.Equals(name,
                                                               StringComparison.OrdinalIgnoreCase)));
    }

    /// <summary>
    /// Adds a new character to the in-memory store.
    /// </summary>
    /// <param name="character">The character to add.</param>
    /// <returns>A completed task.</returns>
    public Task AddAsync(Character character) {
      characters[character.Id] = character;
      return Task.CompletedTask;
    }

    /// <summary>
    /// Updates an existing character in the in-memory store.
    /// </summary>
    /// <param name="character">The character to update.</param>
    /// <returns>A completed task.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the character to update does not exist in the store.</exception>
    public Task UpdateAsync(Character character) {
      if (!characters.ContainsKey(character.Id)) {
        throw new KeyNotFoundException($"Character with ID {character.Id} not found");
      }

      characters[character.Id] = character;
      return Task.CompletedTask;
    }

    /// <summary>
    /// Deletes a character from the in-memory store.
    /// </summary>
    /// <param name="id">The unique identifier of the character to delete.</param>
    /// <returns>A completed task.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the character to delete does not exist in the store.</exception>
    public Task DeleteAsync(Guid id) {
      if (!characters.ContainsKey(id)) {
        throw new KeyNotFoundException($"Character with ID {id} not found");
      }

      characters.Remove(id);
      return Task.CompletedTask;
    }
  }
}
