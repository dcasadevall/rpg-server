using RPGCharacterService.Entities;

namespace RPGCharacterService.Exceptions.Currency {
  /// <summary>
  /// Exception thrown when attempting to perform an invalid currency exchange operation.
  /// This can occur when trying to exchange between the same currency types or when the exchange rate is invalid.
  /// </summary>
  public class InvalidCurrencyExchangeException(CurrencyType from, CurrencyType to, int amount)
    : Exception($"Invalid currency exchange from {amount} {from} to {to}.");
}
