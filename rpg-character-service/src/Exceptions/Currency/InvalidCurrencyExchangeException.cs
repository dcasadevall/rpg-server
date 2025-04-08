using RPGCharacterService.Models;

namespace RPGCharacterService.Exceptions.Currency {
  /// <summary>
  /// Exception thrown when attempting to perform an invalid currency exchange operation.
  /// This can occur when trying to exchange between the same currency types or when the exchange rate is invalid.
  /// </summary>
  public class InvalidCurrencyExchangeException : Exception {
    public InvalidCurrencyExchangeException() : base("Invalid currency exchange.") { }
  }
}
