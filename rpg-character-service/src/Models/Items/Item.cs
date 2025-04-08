namespace RPGCharacterService.Models.Items {
  /// <summary>
  /// Represents an item, which at the moment can be a weapon, armor, or shield.
  /// In the future, this could be expanded to include other item types, such as potions, scrolls, etc.
  /// </summary>
  public record Item {
    /// <summary>
    /// Gets the unique identifier for this item.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Gets the name of the item.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Gets the equipment-specific stats for this item, if it is equipment.
    /// </summary>
    public EquipmentStats? EquipmentStats { get; init; }
  }

  /// <summary>
  /// Contains the equipment-specific statistics for an item, including armor bonuses and type-specific stats.
  /// </summary>
  public record EquipmentStats {
    /// <summary>
    /// Gets the bonus to armor class provided by this equipment.
    /// </summary>
    public int ArmorBonus { get; init; }

    /// <summary>
    /// Gets the type of equipment (Armor, Weapon, or Shield).
    /// </summary>
    public EquipmentType EquipmentType { get; init; }

    /// <summary>
    /// Gets the armor-specific statistics, if this is an armor item.
    /// </summary>
    public ArmorStats? ArmorStats { get; init; }

    /// <summary>
    /// Gets the weapon-specific statistics, if this is a weapon item.
    /// </summary>
    public WeaponStats? WeaponStats { get; init; }
  }

  /// <summary>
  /// Contains the armor-specific statistics for an armor item.
  /// </summary>
  public record ArmorStats {
    /// <summary>
    /// Gets the base armor class value for this armor.
    /// </summary>
    public int BaseArmorClass { get; init; } = 0;

    /// <summary>
    /// Gets the type of armor (Light, Medium, Heavy, or None).
    /// </summary>
    public ArmorType ArmorType { get; init; } = ArmorType.Light;
  }

  /// <summary>
  /// Contains the weapon-specific statistics for a weapon item.
  /// </summary>
  public record WeaponStats {
    /// <summary>
    /// Gets the properties of this weapon (e.g., Finesse, Two-Handed, etc.).
    /// </summary>
    public WeaponProperty WeaponProperties { get; init; } = WeaponProperty.None;

    /// <summary>
    /// Gets the range type of this weapon (Melee or Ranged).
    /// </summary>
    public WeaponRangeType RangeType { get; init; } = WeaponRangeType.Melee;
  }

  /// <summary>
  /// Provides extension methods for checking item types.
  /// </summary>
  public static class EquipmentExtensions {
    /// <summary>
    /// Determines if this item is an armor.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>True if the item is an armor, false otherwise.</returns>
    public static bool IsArmor(this Item item) {
      return item.EquipmentStats?.EquipmentType == EquipmentType.Armor;
    }

    /// <summary>
    /// Determines if this item is a weapon.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>True if the item is a weapon, false otherwise.</returns>
    public static bool IsWeapon(this Item item) {
      return item.EquipmentStats?.EquipmentType == EquipmentType.Weapon;
    }

    /// <summary>
    /// Determines if this item is a shield.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>True if the item is a shield, false otherwise.</returns>
    public static bool IsShield(this Item item) {
      return item.EquipmentStats?.EquipmentType == EquipmentType.Shield;
    }
  }

  /// <summary>
  /// Provides extension methods for weapon-specific checks.
  /// </summary>
  public static class WeaponExtensions {
    /// <summary>
    /// Determines if this item is a two-handed weapon.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>True if the item is a two-handed weapon, false otherwise.</returns>
    public static bool IsTwoHandedWeapon(this Item item) {
      return item.IsWeapon() && ((item.EquipmentStats?.WeaponStats?.WeaponProperties ?? 0) & WeaponProperty.TwoHanded) != 0;
    }
  }
}
