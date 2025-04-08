using RPGCharacterService.Exceptions.Equipment;
using RPGCharacterService.Models.Characters;
using RPGCharacterService.Models.Items;

namespace RPGCharacterService.Models {
  /// <summary>
  /// Represents the items currently equipped by a character, including weapons, armor, and shields.
  /// This class handles calculations related to equipment, such as armor class and weapon damage modifiers.
  /// </summary>
  public class Equipment {
    /// <summary>
    /// Gets the item equipped in the main hand (typically a weapon).
    /// If null, no weapon is equipped.
    /// </summary>
    public Item? MainHand { get; private set; }

    /// <summary>
    /// Gets the item equipped in the off-hand (typically a shield or second weapon).
    /// If null, no item is equipped in the off-hand.
    /// </summary>
    public Item? OffHand { get; private set; }

    /// <summary>
    /// Gets the armor equipped by the character.
    /// If null, no armor is equipped.
    /// </summary>
    public Item? Armor { get; private set; }

    /// <summary>
    /// Calculates the total armor class based on equipped armor and the character's dexterity modifier.
    /// Shields are treated as armor with a bonus to AC.
    /// Does not currently account for magic armor bonus in other items.
    /// </summary>
    /// <param name="dexterityModifier">The character's dexterity modifier.</param>
    /// <returns>The total armor class value, accounting for armor type and dexterity modifier limits.</returns>
    public int CalculateArmorClass(int dexterityModifier) {
      var armorType = Armor?.EquipmentStats?.ArmorStats?.ArmorType ?? ArmorType.None;
      var baseArmorClass = Armor?.EquipmentStats?.ArmorStats?.BaseArmorClass ?? 0;
      var shieldBonus = OffHand?.IsShield() ?? false ? OffHand.EquipmentStats?.ArmorBonus ?? 0 :  0;

      var acBeforeBonus = armorType switch {
        ArmorType.Light => baseArmorClass + dexterityModifier,
        ArmorType.Medium => baseArmorClass + Math.Min(dexterityModifier, 2),
        ArmorType.Heavy => baseArmorClass,
        ArmorType.None => 10 + dexterityModifier,
        _ => throw new NotSupportedException($"Unknown armor type: {armorType}")
      };

      return acBeforeBonus + shieldBonus;
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

    /// <summary>
    /// Equips a weapon in the main hand or off-hand slot.
    /// If a weapon, shield, or two weapons are equipped, the new weapon will replace the old ones.
    /// The way in which old items are unequiped is dependent on the equiped weapon type, as well as
    /// the currently equipped weapons or shield.
    ///
    /// Possible Scenarios:
    /// 1. Two-handed weapon: Equips it in the main hand and unequip any off-hand item
    /// 2. One-handed weapon: Equip it in the main hand or off-hand (depending on the offhand flag)
    ///    2.1: Player has a 2 handed weapon equipped: Unequip it first
    ///    2.2: Player has two one-handed weapons or shield equipped: Unequip the one in the slot we are occupying
    /// </summary>
    /// <param name="item">The weapon item to equip.</param>
    /// <param name="offHand">If true, the weapon will be equipped in the off-hand slot; otherwise, it will be equipped in the main hand.</param>
    /// <exception cref="EquipmentTypeMismatchException">Thrown when the item is not a weapon.</exception>
    /// <exception cref="IllegalEquipmentStateException">If an invalid weapon is equipped in the offhand</exception>
    public void EquipWeapon(Item item, bool offHand = false) {
      if (!item.IsWeapon()) {
        throw new EquipmentTypeMismatchException(item.Id, EquipmentType.Weapon);
      }

      if (item.IsTwoHandedWeapon() && offHand) {
        throw new IllegalEquipmentStateException("Cannot equip a two-handed weapon in the off-hand slot.");
      }

      Item? mainHandItem = MainHand;
      Item? offHandItem = OffHand;

      if (item.IsTwoHandedWeapon()) {
        // Scenario 1: Two-handed weapon
        mainHandItem = item;
        offHandItem = null;
      } else if (mainHandItem != null) {
        // Scenario 2: One-handed weapon
        // Scenario 2.1: Player has a two-handed weapon equipped: Unequip it first
        if (mainHandItem.IsTwoHandedWeapon()) {
          mainHandItem = null;
          offHandItem = null;
        }

        // Scenario 2.2: Take over whichever slot this weapon goes into
        if (!offHand) {
          mainHandItem = item;
        } else {
          offHandItem = item;
        }
      }

      // Update the equipped items
      MainHand = mainHandItem;
      OffHand = offHandItem;
    }

    /// <summary>
    /// Equips a shield in the off-hand slot.
    /// Handles unequipping gear appropiately.
    /// Scenario 1: Player has a two-handed weapon equipped: Unequip it first
    /// Scenario 2: Player has a one-handed weapon or shield equipped in the off-hand: Replace it
    /// </summary>
    /// <param name="item">The shield item to equip.</param>
    /// <exception cref="EquipmentTypeMismatchException">Thrown when the item is not a shield.</exception>
    public void EquipShield(Item item) {
      if (!item.IsShield()) {
        throw new EquipmentTypeMismatchException(item.Id, EquipmentType.Shield);
      }

      if (MainHand?.IsTwoHandedWeapon() ?? false) {
        MainHand = null;
      }

      OffHand = item;
    }

    /// <summary>
    /// Equips the given armor item.
    /// This has no side effects, other than replacing any previously equipped armor.
    /// </summary>
    /// <param name="item">The armor item to equip.</param>
    /// <exception cref="EquipmentTypeMismatchException">Thrown when the item is not a shield.</exception>
    public void EquipArmor(Item item) {
      if (!item.IsArmor()) {
        throw new EquipmentTypeMismatchException(item.Id, EquipmentType.Armor);
      }

      Armor = item;
    }
  }
}
