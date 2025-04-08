using RPGCharacterService.Models.Items;

namespace RPGCharacterService.Models.Characters {
  public class Character {
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; init; }
    public string Race { get; init; }
    public string Subrace { get; init; }
    public string Class { get; init; }
    public int HitPoints { get; set; }
    public int Level { get; set; }
    public CharacterInitializationFlags InitFlags { get; set; } = 0;
    public Dictionary<AbilityScore, int> AbilityScores { get; init; } = new();
    public EquippedItems Equipment { get; init; } = new();
    public Wealth Wealth { get; set; } = new();

    public int CalculateMaxHitPoints() {
      var constitutionModifier = CalculateAbilityModifier(AbilityScore.Constitution);
      return 10 + constitutionModifier * Level;
    }

    public int CalculateProficiencyBonus() {
      return Level switch {
        >= 17 => 6,
        >= 13 => 5,
        >= 9 => 4,
        >= 5 => 3,
        _ => 2
      };
    }

    public int CalculateAbilityModifier(AbilityScore score) {
      return (AbilityScores[score] - 10) / 2;
    }

    public Dictionary<AbilityScore, int> CalculateAllAbilityModifiers() {
      var modifiers = new Dictionary<AbilityScore, int>();
      foreach (var abilityScore in AbilityScores) {
        modifiers[abilityScore.Key] = CalculateAbilityModifier(abilityScore.Key);
      }
      return modifiers;
    }

    public int CalculateArmorClass() {
      var dexterityModifier = CalculateAbilityModifier(AbilityScore.Dexterity);
      return Equipment.CalculateArmorClass(dexterityModifier);
    }

    public AbilityScore CalculateWeaponDamageModifier() {
      var abilityModifiers = CalculateAllAbilityModifiers();
      return Equipment.CalculateWeaponDamageModifier(abilityModifiers);
    }
  }

  public class EquippedItems {
    public Item? MainHand { get; set; }
    public Item? OffHand { get; set; }
    public Item? Armor { get; set; }

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
