using RPGCharacterService.Exceptions.Currency;
using RPGCharacterService.Models;
using RPGCharacterService.Models.Characters;
using RPGCharacterService.Persistence.Characters;

namespace RPGCharacterService.Services {
  public interface ICurrencyService {
    Task<Character> GenerateInitialCurrencyAsync(Guid characterId);
    Task<Character> ModifyCurrenciesAsync(Guid characterId, Dictionary<CurrencyType, int> currencyChanges);
    Task<Character> ExchangeCurrencyAsync(Guid characterId, CurrencyType from, CurrencyType to, int amount);
  }

  public class CurrencyService(ICharacterRepository repository, IDiceService diceService) : ICurrencyService {
    public async Task<Character> GenerateInitialCurrencyAsync(Guid characterId) {
      var character = await repository.GetByIdOrThrowAsync(characterId);

      if (character.InitFlags.HasFlag(CharacterInitializationFlags.CurrencyInitialized)) {
        throw new CurrencyAlreadyInitializedException(characterId);
      }

      // Generate currency using dice rolls
      var goldAmount = diceService
                       .Roll(DiceSides.Twenty, 1)
                       .Sum();
      var silverAmount = diceService
                         .Roll(DiceSides.Twenty, 3)
                         .Sum();
      var copperAmount = diceService
                         .Roll(DiceSides.Twelve, 5)
                         .Sum();

      character.Wealth.SetCurrencyAmount(CurrencyType.Gold, goldAmount);
      character.Wealth.SetCurrencyAmount(CurrencyType.Silver, silverAmount);
      character.Wealth.SetCurrencyAmount(CurrencyType.Copper, copperAmount);
      character.InitFlags |= CharacterInitializationFlags.CurrencyInitialized;

      await repository.UpdateAsync(character);
      return character;
    }

    public async Task<Character> ModifyCurrenciesAsync(Guid characterId, Dictionary<CurrencyType, int> currencyChanges) {
      var character = await repository.GetByIdOrThrowAsync(characterId);

      if (!character.InitFlags.HasFlag(CharacterInitializationFlags.CurrencyInitialized)) {
        throw new CurrencyNotInitializedException(characterId);
      }

      // Apply changes
      foreach (var change in currencyChanges) {
        var currencyAmountAfter = character.Wealth.GetCurrencyAmount(change.Key) - change.Value;
        if (currencyAmountAfter < 0) {
          throw new NotEnoughCurrencyException(change.Key,
                                               -change.Value,
                                               character.Wealth.GetCurrencyAmount(change.Key));
        }
      }

      await repository.UpdateAsync(character);
      return character;
    }

    public async Task<Character> ExchangeCurrencyAsync(Guid characterId, CurrencyType from, CurrencyType to, int amount) {
      var character = await repository.GetByIdOrThrowAsync(characterId);
      if (!character.InitFlags.HasFlag(CharacterInitializationFlags.CurrencyInitialized)) {
        throw new CurrencyNotInitializedException(characterId);
      }

      // Check if character has enough currency to exchange
      var currentAmount = character.Wealth.GetCurrencyAmount(from);
      if (currentAmount < amount) {
        throw new NotEnoughCurrencyException(from, amount, currentAmount);
      }

      // Calculate exchange rate
      var exchangeRate = GetExchangeRate(from, to);
      var newAmount = amount * exchangeRate;

      // Update currency amounts
      character.Wealth.SetCurrencyAmount(from, currentAmount - amount);
      character.Wealth.SetCurrencyAmount(to,
        character.Wealth.GetCurrencyAmount(to) + newAmount);

      await repository.UpdateAsync(character);
      return character;
    }

    private static int GetExchangeRate(CurrencyType from, CurrencyType to) {
      // Standard D&D 5e conversion rates
      return (from, to) switch {
        (CurrencyType.Copper, CurrencyType.Silver) => 1,
        (CurrencyType.Silver, CurrencyType.Copper) => 10,
        (CurrencyType.Silver, CurrencyType.Electrum) => 1,
        (CurrencyType.Electrum, CurrencyType.Silver) => 5,
        (CurrencyType.Electrum, CurrencyType.Gold) => 1,
        (CurrencyType.Gold, CurrencyType.Electrum) => 2,
        (CurrencyType.Gold, CurrencyType.Platinum) => 1,
        (CurrencyType.Platinum, CurrencyType.Gold) => 10,
        _ => throw new InvalidOperationException($"Invalid currency conversion from {from} to {to}")
      };
    }
  }
}
