namespace RPGCharacterService.Models.Characters {
  /// <summary>
  /// Represents the six ability scores.
  /// These scores determine a character's basic capabilities and affect various game mechanics.
  /// </summary>
  public enum AbilityScore {
    /// <summary>
    /// Measures physical power and athletic ability.
    /// Affects melee attacks, carrying capacity, and strength-based skills.
    /// </summary>
    Strength,

    /// <summary>
    /// Measures agility, reflexes, and balance.
    /// Affects ranged attacks, armor class, and dexterity-based skills.
    /// </summary>
    Dexterity,

    /// <summary>
    /// Measures health, stamina, and vital force.
    /// Affects hit points and constitution-based skills.
    /// </summary>
    Constitution,

    /// <summary>
    /// Measures mental acuity, information recall, and analytical skill.
    /// Affects spellcasting for some classes and intelligence-based skills.
    /// </summary>
    Intelligence,

    /// <summary>
    /// Measures awareness, intuition, and insight.
    /// Affects perception and wisdom-based skills.
    /// </summary>
    Wisdom,

    /// <summary>
    /// Measures force of personality, persuasiveness, and leadership.
    /// Affects social interactions and charisma-based skills.
    /// </summary>
    Charisma
  }
}
