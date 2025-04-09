namespace RPGCharacterService.Entities.Items {
  /// <summary>
  /// Represents the different types of armor that can be equipped by characters.
  /// Note: Shields are not included in this enum as they are treated as a separate equipment type,
  /// unlike in D&D where they are considered a type of armor.
  /// </summary>
  public enum ArmorType {
    /// <summary>
    /// No armor equipped. Character uses their base armor class.
    /// </summary>
    None,

    /// <summary>
    /// Light armor that allows full use of dexterity modifier for armor class.
    /// Examples include padded, leather, and studded leather armor.
    /// </summary>
    Light,

    /// <summary>
    /// Medium armor that allows limited use of dexterity modifier (maximum +2) for armor class.
    /// Examples include hide, chain shirt, and scale mail.
    /// </summary>
    Medium,

    /// <summary>
    /// Heavy armor that does not allow dexterity modifier to be added to armor class.
    /// Examples include ring mail, chain mail, splint, and plate armor.
    /// </summary>
    Heavy
  }
}
