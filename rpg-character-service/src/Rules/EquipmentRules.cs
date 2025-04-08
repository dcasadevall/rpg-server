using RPGCharacterService.Models.Characters;
using RPGCharacterService.Models.Items;

namespace RPGCharacterService.Rules {
  public interface IEquipmentRules {
    int CalculateArmorClass(Character character);
    AbilityScore CalculateWeaponDamageModifier(Character character);
  }

  public class DndFifthEditionEquipmentRules(ICharacterRules characterRules) : IEquipmentRules {
    public int CalculateArmorClass(Character character) {
      var abilityModifiers = characterRules.CalculateAbilityModifiers(character);
      var armorType = character.Equipment.Armor?.EquipmentStats?.ArmorStats?.ArmorType ?? ArmorType.None;
      var baseArmorClass = character.Equipment.Armor?.EquipmentStats?.ArmorStats?.BaseArmorClass ?? 0;
      var dexterityModifier = abilityModifiers[AbilityScore.Dexterity];

      return armorType switch {
        ArmorType.Light => baseArmorClass + dexterityModifier,
        ArmorType.Medium => baseArmorClass + Math.Min(dexterityModifier, 2),
        ArmorType.Heavy => baseArmorClass,
        ArmorType.None => 10 + dexterityModifier,
        _ => throw new NotSupportedException($"Unknown armor type: {armorType}")
      };
    }

    public AbilityScore CalculateWeaponDamageModifier(Character character) {
      var abilityModifiers = characterRules.CalculateAbilityModifiers(character);
      var strengthModifier = abilityModifiers[AbilityScore.Strength];
      var dexterityModifier = abilityModifiers[AbilityScore.Dexterity];

      // 1. Finesse weapon: player chooses best of Strength or Dexterity
      if (character.Equipment.MainHand?.EquipmentStats?.WeaponStats?.WeaponProperties.HasFlag(WeaponProperty.Finesse) ?? false) {
        return dexterityModifier > strengthModifier ? AbilityScore.Dexterity : AbilityScore.Strength;
      }

      // 2. Ranged weapon (non-finesse): use Dexterity
      if (character.Equipment.MainHand?.EquipmentStats?.WeaponStats?.RangeType == WeaponRangeType.Ranged) {
        return AbilityScore.Dexterity;
      }

      // 3. Otherwise: melee weapon uses Strength
      return AbilityScore.Strength;
    }
  }
}
