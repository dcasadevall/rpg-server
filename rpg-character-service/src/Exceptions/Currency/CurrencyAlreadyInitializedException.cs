namespace RPGCharacterService.Exceptions.Currency {
  /// <summary>
  /// Exception thrown when attempting to initialize currency for a character that already has currency initialized.
  /// Currency initialization is a one-time operation per character and cannot be performed multiple times.
  /// </summary>
  /// <param name="characterId">The unique identifier of the character whose currency is already initialized.</param>
  public class CurrencyAlreadyInitializedException(Guid characterId)
    : Exception($"Currency has already been initialized for character with ID {characterId}.");
}
