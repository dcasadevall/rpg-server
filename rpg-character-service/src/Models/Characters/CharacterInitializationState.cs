namespace RPGCharacterService.Models.Characters {
  [Flags]
  public enum CharacterInitializationFlags {
    // No initialization has been done
    None = 0,
    // Initial currency has been set
    CurrencyInitialized = 1 << 0
  }
}
