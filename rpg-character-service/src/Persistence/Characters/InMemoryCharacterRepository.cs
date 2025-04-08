using RPGCharacterService.Models.Characters;

namespace RPGCharacterService.Persistence.Characters {
  public class InMemoryCharacterRepository : ICharacterRepository {
    private readonly Dictionary<Guid, Character> characters = new();

    public Task<IEnumerable<Character>> GetAllAsync() {
      return Task.FromResult(characters.Values.AsEnumerable());
    }

    public Task<Character?> GetByIdAsync(Guid id) {
      return Task.FromResult(characters.GetValueOrDefault(id));
    }

    public Task<Character?> GetByNameAsync(string name) {
      return Task.FromResult(characters.Values.FirstOrDefault(c => c.Name.Equals(name,
                                                               StringComparison.OrdinalIgnoreCase)));
    }

    public Task AddAsync(Character character) {
      characters[character.Id] = character;
      return Task.CompletedTask;
    }

    public Task UpdateAsync(Character character) {
      if (!characters.ContainsKey(character.Id)) {
        throw new KeyNotFoundException($"Character with ID {character.Id} not found");
      }

      characters[character.Id] = character;
      return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id) {
      if (!characters.ContainsKey(id)) {
        throw new KeyNotFoundException($"Character with ID {id} not found");
      }

      characters.Remove(id);
      return Task.CompletedTask;
    }
  }
}
