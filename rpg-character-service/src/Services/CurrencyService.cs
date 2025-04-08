using RPGCharacterService.Exceptions.Currency;
using RPGCharacterService.Models;
using RPGCharacterService.Models.Characters;
using RPGCharacterService.Persistence.Characters;

namespace RPGCharacterService.Services {
  /// <summary>
  /// Interface defining the contract for character currency management operations.
  /// Handles initialization, modification, and exchange of different currency types.
  /// </summary>
  public interface ICurrencyService {
    /// <summary>
    /// Generates initial currency for a character using random dice rolls.
    /// This operation can only be performed once per character.
    /// </summary>
    /// <param name="characterId">The unique identifier of the character.</param>
    /// <returns>The character with initialized currency amounts.</returns>
    /// <exception cref="CurrencyAlreadyInitializedException">Thrown when currency has already been initialized for the character.</exception>
    Task<Character> GenerateInitialCurrencyAsync(Guid characterId);

    /// <summary>
    /// Modifies a character's currency amounts by adding or subtracting specified values.
    /// </summary>
    /// <param name="characterId">The unique identifier of the character.</param>
    /// <param name="currencyChanges">Dictionary of currency types and their respective changes (positive for adding, negative for subtracting).</param>
    /// <returns>The character with updated currency amounts.</returns>
    /// <exception cref="CurrencyNotInitializedException">Thrown when currency has not been initialized for the character.</exception>
    /// <exception cref="NotEnoughCurrencyException">Thrown when the character doesn't have enough currency for the requested changes.</exception>
    Task<Character> ModifyCurrenciesAsync(Guid characterId, Dictionary<CurrencyType, int> currencyChanges);

    /// <summary>
    /// Exchanges one type of currency for another using standard D&D 5e conversion rates.
    /// </summary>
    /// <param name="characterId">The unique identifier of the character.</param>
    /// <param name="from">The currency type to exchange from.</param>
    /// <param name="to">The currency type to exchange to.</param>
    /// <param name="amount">The amount of currency to exchange.</param>
    /// <returns>The character with updated currency amounts after the exchange.</returns>
    /// <exception cref="CurrencyNotInitializedException">Thrown when currency has not been initialized for the character.</exception>
    /// <exception cref="NotEnoughCurrencyException">Thrown when the character doesn't have enough currency for the exchange.</exception>
    /// <exception cref="InvalidCurrencyExchangeException">Thrown when attempting an invalid currency exchange.</exception>
    Task<Character> ExchangeCurrencyAsync(Guid characterId, CurrencyType from, CurrencyType to, int amount);
  }

  /// <summary>
  /// Service implementation for managing character currency operations.
  /// Handles currency initialization, modification, and exchange using standard D&D 5e rules.
  /// </summary>
  public class CurrencyService(ICharacterRepository repository, IDiceService diceService) : ICurrencyService {
    /// <summary>
    /// Generates initial currency for a character using random dice rolls.
    /// The amounts are determined by rolling different dice combinations:
    /// - Gold: 1d20
    /// - Silver: 3d20
    /// - Copper: 5d12
    /// </summary>
    /// <param name="characterId">The unique identifier of the character.</param>
    /// <returns>The character with initialized currency amounts.</returns>
    /// <exception cref="CurrencyAlreadyInitializedException">Thrown when currency has already been initialized for the character.</exception>
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

    /// <summary>
    /// Modifies a character's currency amounts by adding or subtracting specified values.
    /// Ensures the character has sufficient funds before applying negative changes.
    /// </summary>
    /// <param name="characterId">The unique identifier of the character.</param>
    /// <param name="currencyChanges">Dictionary of currency types and their respective changes.</param>
    /// <returns>The character with updated currency amounts.</returns>
    /// <exception cref="CurrencyNotInitializedException">Thrown when currency has not been initialized for the character.</exception>
    /// <exception cref="NotEnoughCurrencyException">Thrown when the character doesn't have enough currency for the requested changes.</exception>
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

    /// <summary>
    /// Exchanges one type of currency for another using standard D&D 5e conversion rates.
    /// The exchange rates are:
    /// - 10 copper = 1 silver
    /// - 5 silver = 1 electrum
    /// - 2 electrum = 1 gold
    /// - 10 gold = 1 platinum
    /// </summary>
    /// <param name="characterId">The unique identifier of the character.</param>
    /// <param name="from">The currency type to exchange from.</param>
    /// <param name="to">The currency type to exchange to.</param>
    /// <param name="amount">The amount of currency to exchange.</param>
    /// <returns>The character with updated currency amounts after the exchange.</returns>
    /// <exception cref="CurrencyNotInitializedException">Thrown when currency has not been initialized for the character.</exception>
    /// <exception cref="NotEnoughCurrencyException">Thrown when the character doesn't have enough currency for the exchange.</exception>
    /// <exception cref="InvalidCurrencyExchangeException">Thrown when attempting an invalid currency exchange.</exception>
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

    /// <summary>
    /// Gets the exchange rate between two currency types based on standard D&D 5e conversion rates.
    /// </summary>
    /// <param name="from">The currency type to exchange from.</param>
    /// <param name="to">The currency type to exchange to.</param>
    /// <returns>The exchange rate multiplier.</returns>
    /// <exception cref="InvalidCurrencyExchangeException">Thrown when attempting an invalid currency exchange.</exception>
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
        _ => throw new InvalidCurrencyExchangeException(from, to)
      };
    }
  }
}
