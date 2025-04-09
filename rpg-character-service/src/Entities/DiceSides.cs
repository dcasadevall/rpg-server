namespace RPGCharacterService.Entities {
  /// <summary>
  /// Represents the standard dice types used in tabletop role-playing games.
  /// Each value corresponds to the number of sides on the die.
  /// </summary>
  public enum DiceSides {
    /// <summary>
    /// A four-sided die (d4), commonly used for small damage rolls or minor effects.
    /// </summary>
    Four = 4,

    /// <summary>
    /// A six-sided die (d6), the most common die type used for various rolls including damage and ability checks.
    /// </summary>
    Six = 6,

    /// <summary>
    /// An eight-sided die (d8), often used for weapon damage and spell effects.
    /// </summary>
    Eight = 8,

    /// <summary>
    /// A ten-sided die (d10), used for percentile rolls (when paired with another d10) and certain damage types.
    /// </summary>
    Ten = 10,

    /// <summary>
    /// A twelve-sided die (d12), typically used for larger weapon damage rolls.
    /// </summary>
    Twelve = 12,

    /// <summary>
    /// A twenty-sided die (d20), the primary die used for attack rolls, saving throws, and ability checks.
    /// </summary>
    Twenty = 20
  }
}
