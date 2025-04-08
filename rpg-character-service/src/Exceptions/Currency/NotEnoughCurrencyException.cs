using RPGCharacterService.Models;

namespace RPGCharacterService.Exceptions.Currency {
  public class NotEnoughCurrencyException(CurrencyType currencyType, int requiredAmount, int availableAmount)
    : Exception($"Not enough {currencyType} currency available. " +
                $"Required: {requiredAmount}, Available: {availableAmount}") {
  }
}
