using RPGCharacterService.Models.Characters;
using RPGCharacterService.Persistence.Characters;

namespace RPGCharacterService.Services {
  public interface IStatsService {
    Task<Character> ModifyHitPointsAsync(Guid characterId, int delta);
  }

  public class StatsService(ICharacterRepository repository) : IStatsService {
    public async Task<Character> ModifyHitPointsAsync(Guid characterId, int delta) {
      var character = await repository.GetByIdOrThrowAsync(characterId);
      var maxHitPoints = character.CalculateMaxHitPoints();
      character.HitPoints = Math.Clamp(character.HitPoints + delta, 0, maxHitPoints);
      await repository.UpdateAsync(character);

      return character;
    }
  }
}
