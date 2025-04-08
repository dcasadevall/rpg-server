namespace RPGCharacterService.Models.Characters {
  /// <summary>
  /// Flags used to track the initialization state of a character.
  /// This is useful because the character creation process requires multiple stages
  /// initiated from the client.
  /// </summary>
  [Flags]
  public enum CharacterInitializationFlags {
    // No initialization has been done
    None = 0,
    // Initial currency has been set
    CurrencyInitialized = 1 << 0
  }
}
