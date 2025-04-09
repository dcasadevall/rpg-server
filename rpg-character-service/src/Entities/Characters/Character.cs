using RPGCharacterService.Entities.Items;

namespace RPGCharacterService.Entities.Characters {
  /// <summary>
  /// Represents a D&amp;D character, containing all character attributes, equipment, and wealth.
  /// This class implements core D&amp;D 5e character mechanics including hit points, ability scores, and armor class calculations.
  /// This model is an Aggregate Root, containing other models such as Equipment or Wealth.
  /// </summary>
  public class Character {
    /// <summary>
    /// Gets the unique identifier for this character.
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Gets or initializes the character's name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets or initializes the character's race (e.g., Human, Elf, Dwarf).
    /// </summary>
    public required string Race { get; init; }

    /// <summary>
    /// Gets or initializes the character's subrace (e.g., High, Mountain, Deep, etc.).
    /// </summary>
    public required string Subrace { get; init; }

    /// <summary>
    /// Gets or initializes the character's class (e.g., Fighter, Wizard, Rogue).
    /// </summary>
    public required string Class { get; init; }

    /// <summary>
    /// Gets or sets the character's current hit points.
    /// </summary>
    public int HitPoints { get; set; }

    /// <summary>
    /// Gets or sets the character's level, which affects proficiency bonus and other level-dependent calculations.
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Gets or sets the initialization flags that track which aspects of the character have been initialized.
    /// </summary>
    public CharacterInitializationFlags InitFlags { get; set; } = 0;

    /// <summary>
    /// Gets or initializes the character's ability scores (Strength, Dexterity, Constitution, Intelligence, Wisdom, Charisma).
    /// </summary>
    public Dictionary<AbilityScore, int> AbilityScores { get; init; } = new();

    /// <summary>
    /// Gets or initializes the character's equipped items (weapons, armor, shield).
    /// </summary>
    public Equipment Equipment { get; init; } = new();

    /// <summary>
    /// Gets or sets the character's wealth in various currency types (gold, silver, copper, etc.).
    /// </summary>
    public Wealth Wealth { get; init; } = new();

    /// <summary>
    /// Calculates the character's maximum hit points based on their constitution modifier and level.
    /// </summary>
    /// <returns>The maximum hit points the character can have at their current level.</returns>
    public int CalculateMaxHitPoints() {
      var constitutionModifier = CalculateAbilityModifier(AbilityScore.Constitution);
      return 10 + constitutionModifier * Level;
    }

    /// <summary>
    /// Calculates the character's proficiency bonus based on their level.
    /// </summary>
    /// <returns>The proficiency bonus value according to D&amp;D 5e rules.</returns>
    public int CalculateProficiencyBonus() {
      return Level switch {
        >= 17 => 6,
        >= 13 => 5,
        >= 9 => 4,
        >= 5 => 3,
        _ => 2
      };
    }

    /// <summary>
    /// Calculates the ability modifier for a given ability score.
    /// </summary>
    /// <param name="score">The ability score to calculate the modifier for.</param>
    /// <returns>The ability modifier, calculated as (score - 10) / 2, rounded down.</returns>
    public int CalculateAbilityModifier(AbilityScore score) {
      return (int)Math.Floor((AbilityScores[score] - 10) / 2.0);
    }

    /// <summary>
    /// Calculates ability modifiers for all the character's ability scores.
    /// </summary>
    /// <returns>A dictionary mapping each ability score to its corresponding modifier.</returns>
    public Dictionary<AbilityScore, int> CalculateAllAbilityModifiers() {
      var modifiers = new Dictionary<AbilityScore, int>();
      foreach (var abilityScore in AbilityScores) {
        modifiers[abilityScore.Key] = CalculateAbilityModifier(abilityScore.Key);
      }
      return modifiers;
    }

    /// <summary>
    /// Calculates the character's armor class based on their equipment and dexterity modifier.
    /// </summary>
    /// <returns>The total armor class value.</returns>
    public int CalculateArmorClass() {
      var dexterityModifier = CalculateAbilityModifier(AbilityScore.Dexterity);
      return Equipment.CalculateArmorClass(dexterityModifier);
    }

    /// <summary>
    /// Determines which ability score (Strength or Dexterity) should be used for weapon damage calculations.
    /// </summary>
    /// <returns>The ability score that should be used for weapon damage calculations.</returns>
    public AbilityScore CalculateWeaponDamageModifier() {
      var abilityModifiers = CalculateAllAbilityModifiers();
      return Equipment.CalculateWeaponDamageModifier(abilityModifiers);
    }
  }

}
