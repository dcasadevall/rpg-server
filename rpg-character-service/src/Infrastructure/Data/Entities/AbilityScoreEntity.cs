using RPGCharacterService.Models.Characters;

namespace RPGCharacterService.Infrastructure.Data.Entities {
  /// <summary>
  /// Database entity representing a character's ability score.
  /// </summary>
  public class AbilityScoreEntity {
    /// <summary>
    /// The unique identifier for this ability score.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The ID of the character this ability score belongs to.
    /// </summary>
    public Guid CharacterId { get; set; }

    /// <summary>
    /// The type of ability (Strength, Dexterity, etc.).
    /// </summary>
    public int AbilityType { get; set; }

    /// <summary>
    /// The ability score value.
    /// </summary>
    public int Score { get; set; }

    // Navigation property

    /// <summary>
    /// The character this ability score belongs to.
    /// </summary>
    public CharacterEntity Character { get; set; } = null!;

    /// <summary>
    /// Gets the ability score type as an enum.
    /// </summary>
    public AbilityScore GetAbilityType() => (AbilityScore) AbilityType;

    /// <summary>
    /// Sets the ability score type from an enum.
    /// </summary>
    public void SetAbilityType(AbilityScore abilityType) => AbilityType = (int) abilityType;
  }
}
