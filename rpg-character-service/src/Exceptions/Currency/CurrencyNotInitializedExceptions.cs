namespace RPGCharacterService.Exceptions.Currency {
  public class CurrencyNotInitializedException(Guid characterId)
    : Exception($"Currency not initialized for character with ID {characterId}.");
}
