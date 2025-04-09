using RPGCharacterService.Entities.Characters;
using RPGCharacterService.Persistence;

namespace RPGCharacterService.Services {
  /// <summary>
  /// Interface defining the contract for character statistics such as hit points or
  /// in the future, experience points.
  /// </summary>
  public interface IStatsService {
    /// <summary>
    /// Modifies a character's hit points by a specified amount, ensuring the value stays within valid bounds.
    /// </summary>
    /// <param name="characterId">The unique identifier of the character.</param>
    /// <param name="delta">The amount to modify the hit points by (positive for healing, negative for damage).</param>
    /// <returns>The updated character with modified hit points.</returns>
    Task<Character> ModifyHitPointsAsync(Guid characterId, int delta);
  }

  /// <summary>
  /// Service responsible for managing character statistics and derived properties.
  /// Handles operations related to hit points, ability scores, and other character attributes.
  /// </summary>
  public class StatsService(ICharacterRepository repository) : IStatsService {
    /// <summary>
    /// Modifies a character's hit points by a specified amount, ensuring the value stays within valid bounds.
    /// The hit points will be clamped between 0 and the character's maximum hit points.
    /// </summary>
    /// <param name="characterId">The unique identifier of the character.</param>
    /// <param name="delta">The amount to modify the hit points by (positive for healing, negative for damage).</param>
    /// <returns>The updated character with modified hit points.</returns>
    public async Task<Character> ModifyHitPointsAsync(Guid characterId, int delta) {
      var character = await repository.GetByIdOrThrowAsync(characterId);
      var maxHitPoints = character.CalculateMaxHitPoints();
      character.HitPoints = Math.Clamp(character.HitPoints + delta, 0, maxHitPoints);
      await repository.UpdateAsync(character);

      return character;
    }
  }
}
