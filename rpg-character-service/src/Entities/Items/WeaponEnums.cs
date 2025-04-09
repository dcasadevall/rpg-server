namespace RPGCharacterService.Entities.Items {
  /// <summary>
  /// Represents the properties that a weapon can have.
  /// These properties affect how the weapon can be used and what modifiers apply to it.
  /// </summary>
  [Flags]
  public enum WeaponProperty {
    /// <summary>
    /// No special properties.
    /// </summary>
    None = 0,

    /// <summary>
    /// Light weapons are easier to wield and can be used for two-weapon fighting.
    /// </summary>
    Light = 1 << 0, // 1

    /// <summary>
    /// Heavy weapons are more difficult to wield and have special rules for small creatures.
    /// </summary>
    Heavy = 1 << 1, // 2

    /// <summary>
    /// Two-handed weapons require both hands to wield and cannot be used with a shield.
    /// </summary>
    TwoHanded = 1 << 2, // 4

    /// <summary>
    /// Versatile weapons can be wielded with one or two hands, with different damage dice.
    /// </summary>
    Versatile = 1 << 3, // 8

    /// <summary>
    /// Thrown weapons can be used for ranged attacks.
    /// </summary>
    Thrown = 1 << 4, // 16

    /// <summary>
    /// Finesse weapons can use either Strength or Dexterity for attack and damage rolls.
    /// </summary>
    Finesse = 1 << 5, // 32

    /// <summary>
    /// Loading weapons require an action to reload after firing.
    /// </summary>
    Loading = 1 << 6, // 64

    /// <summary>
    /// Ammunition weapons require ammunition to be used.
    /// </summary>
    Ammunition = 1 << 7 // 128
  }

  /// <summary>
  /// Represents the range type of a weapon.
  /// </summary>
  public enum WeaponRangeType {
    /// <summary>
    /// Melee weapons are used in close combat.
    /// </summary>
    Melee,

    /// <summary>
    /// Ranged weapons are used at a distance.
    /// </summary>
    Ranged
  }
}
