using RPGCharacterService.Entities;

namespace RPGCharacterService.Exceptions.Currency {
  /// <summary>
  /// Exception thrown when attempting to perform a transaction with insufficient currency.
  /// </summary>
  public class NotEnoughCurrencyException(CurrencyType currencyType, int requiredAmount, int availableAmount)
    : Exception($"Not enough {currencyType} currency available. " +
                $"Required: {requiredAmount}, Available: {availableAmount}") {
  }
}
