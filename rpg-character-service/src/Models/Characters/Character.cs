using RPGCharacterService.Models.Items;

namespace RPGCharacterService.Models.Characters {
  /// <summary>
  /// Represents a D&D character, containing all character attributes, equipment, and wealth.
  /// This class implements core D&D 5e character mechanics including hit points, ability scores, and armor class calculations.
  /// </summary>
  public class Character {
    /// <summary>
    /// Gets the unique identifier for this character.
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    /// Gets or initializes the character's name.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Gets or initializes the character's race (e.g., Human, Elf, Dwarf).
    /// </summary>
    public string Race { get; init; }

    /// <summary>
    /// Gets or initializes the character's subrace (e.g., High, Mountain, Deep, etc.).
    /// </summary>
    public string Subrace { get; init; }

    /// <summary>
    /// Gets or initializes the character's class (e.g., Fighter, Wizard, Rogue).
    /// </summary>
    public string Class { get; init; }

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
    public EquippedItems Equipment { get; init; } = new();

    /// <summary>
    /// Gets or sets the character's wealth in various currency types (gold, silver, copper, etc.).
    /// </summary>
    public Wealth Wealth { get; set; } = new();

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
    /// <returns>The proficiency bonus value according to D&D 5e rules.</returns>
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
      return (AbilityScores[score] - 10) / 2;
    }

    /// <summary>
    /// Calculates ability modifiers for all of the character's ability scores.
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

  /// <summary>
  /// Represents the items currently equipped by a character, including weapons, armor, and shields.
  /// This class handles calculations related to equipment, such as armor class and weapon damage modifiers.
  /// </summary>
  public class EquippedItems {
    /// <summary>
    /// Gets or sets the item equipped in the main hand (typically a weapon).
    /// </summary>
    public Item? MainHand { get; set; }

    /// <summary>
    /// Gets or sets the item equipped in the off-hand (typically a shield or second weapon).
    /// </summary>
    public Item? OffHand { get; set; }

    /// <summary>
    /// Gets or sets the armor equipped by the character.
    /// </summary>
    public Item? Armor { get; set; }

    /// <summary>
    /// Calculates the total armor class based on equipped armor and the character's dexterity modifier.
    /// </summary>
    /// <param name="dexterityModifier">The character's dexterity modifier.</param>
    /// <returns>The total armor class value, accounting for armor type and dexterity modifier limits.</returns>
    public int CalculateArmorClass(int dexterityModifier) {
      var armorType = Armor?.EquipmentStats?.ArmorStats?.ArmorType ?? ArmorType.None;
      var baseArmorClass = Armor?.EquipmentStats?.ArmorStats?.BaseArmorClass ?? 0;
      var armorBonus = Armor?.EquipmentStats?.ArmorBonus ?? 0;

      var acBeforeBonus = armorType switch {
        ArmorType.Light => baseArmorClass + dexterityModifier,
        ArmorType.Medium => baseArmorClass + Math.Min(dexterityModifier, 2),
        ArmorType.Heavy => baseArmorClass,
        ArmorType.None => 10 + dexterityModifier,
        _ => throw new NotSupportedException($"Unknown armor type: {armorType}")
      };

      return acBeforeBonus + armorBonus;
    }

    /// <summary>
    /// Determines which ability score should be used for weapon damage calculations based on weapon properties.
    /// </summary>
    /// <param name="abilityModifiers">A dictionary of the character's ability modifiers.</param>
    /// <returns>The ability score (Strength or Dexterity) that should be used for weapon damage calculations.</returns>
    public AbilityScore CalculateWeaponDamageModifier(Dictionary<AbilityScore, int> abilityModifiers) {
      var strengthModifier = abilityModifiers[AbilityScore.Strength];
      var dexterityModifier = abilityModifiers[AbilityScore.Dexterity];

      if (MainHand?.EquipmentStats?.WeaponStats?.WeaponProperties.HasFlag(WeaponProperty.Finesse) ?? false) {
        return dexterityModifier > strengthModifier ? AbilityScore.Dexterity : AbilityScore.Strength;
      }

      if (MainHand?.EquipmentStats?.WeaponStats?.RangeType == WeaponRangeType.Ranged) {
        return AbilityScore.Dexterity;
      }

      return AbilityScore.Strength;
    }
  }
}
