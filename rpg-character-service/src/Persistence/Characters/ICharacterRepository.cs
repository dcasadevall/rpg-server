using RPGCharacterService.Exceptions.Character;
using RPGCharacterService.Models.Characters;

namespace RPGCharacterService.Persistence.Characters {
  public interface ICharacterRepository {
    Task<IEnumerable<Character>> GetAllAsync();
    Task<Character?> GetByIdAsync(Guid id);
    Task<Character?> GetByNameAsync(string name);
    Task AddAsync(Character character);
    Task UpdateAsync(Character character);
    Task DeleteAsync(Guid id);
  }

  public static class CharacterRepositoryExtensions {
    public static async Task<Character> GetByIdOrThrowAsync(this ICharacterRepository repository, Guid id) {
      var character = await repository.GetByIdAsync(id);
      if (character == null) {
        throw new CharacterNotFoundException(id);
      }

      return character;
    }
  }
}
