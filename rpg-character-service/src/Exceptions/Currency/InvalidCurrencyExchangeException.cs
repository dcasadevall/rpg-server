using RPGCharacterService.Models;

namespace RPGCharacterService.Exceptions.Currency
{
    public class InvalidCurrencyExchangeException(CurrencyType from, CurrencyType to)
        : Exception($"Invalid currency exchange from {from} to {to}.");
}
