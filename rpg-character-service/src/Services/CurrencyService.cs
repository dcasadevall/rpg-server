using RPGCharacterService.Exceptions.Currency;
using RPGCharacterService.Models;
using RPGCharacterService.Models.Characters;
using RPGCharacterService.Persistence.Characters;
using System;
using RPGCharacterService.Exceptions.Character; // Needed for Math and ArgumentOutOfRangeException

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
    /// <exception cref="CharacterNotFoundException">Thrown when the character is not found.</exception>
    /// <exception cref="CurrencyNotInitializedException">Thrown when currency has not been initialized for the character.</exception>
    /// <exception cref="OverflowException">Thrown if the resulting 'to' balance exceeds integer limits.</exception>
    /// <exception cref="NotEnoughCurrencyException">Thrown when the character doesn't have enough currency for the requested changes.</exception>
    Task<Character> ModifyCurrenciesAsync(Guid characterId, Dictionary<CurrencyType, int> currencyChanges);

    /// <summary>
    /// Exchanges a specified amount of one currency ('from') into another ('to')
    /// using standard D&D 5e conversion rates. Fractional results are not converted.
    /// </summary>
    /// <param name="characterId">The unique identifier of the character.</param>
    /// <param name="from">The currency type to exchange from.</param>
    /// <param name="to">The currency type to exchange to.</param>
    /// <param name="amount">The amount of 'from' currency to exchange.</param>
    /// <returns>The character with updated currency amounts.</returns>
    /// <exception cref="CharacterNotFoundException">Thrown when the character is not found.</exception>
    /// <exception cref="CurrencyNotInitializedException">...</exception>
    /// <exception cref="NotEnoughCurrencyException">Thrown when the character doesn't have enough 'from' currency.</exception>
    /// <exception cref="InvalidCurrencyExchangeException">Thrown for invalid exchanges (e.g., same type).</exception>
    /// <exception cref="OverflowException">Thrown if the resulting 'to' balance exceeds integer limits.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if amount is negative.</exception>
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
    public async Task<Character>
      ModifyCurrenciesAsync(Guid characterId, Dictionary<CurrencyType, int> currencyChanges) {
      var character = await repository.GetByIdOrThrowAsync(characterId);

      if (!character.InitFlags.HasFlag(CharacterInitializationFlags.CurrencyInitialized)) {
        throw new CurrencyNotInitializedException(characterId);
      }

      // Apply changes
      foreach (var change in currencyChanges) {
        var currentAmount = character.Wealth.GetCurrencyAmount(change.Key);
        // Check for overflow before applying changes
        if (change.Value > 0 && int.MaxValue - change.Value < currentAmount) {
          throw new OverflowException($"Resulting amount of {change.Key} exceeds integer limits.");
        }

        // Check for negative balance
        var currencyAmountAfter = currentAmount + change.Value;
        if (currencyAmountAfter < 0) {
          throw new NotEnoughCurrencyException(change.Key,
                                               change.Value,
                                               currentAmount);
        }

        // Apply the change
        character.Wealth.SetCurrencyAmount(change.Key, currencyAmountAfter);
      }

      await repository.UpdateAsync(character);
      return character;
    }

    /// <summary>
    /// Exchanges a specified amount of one currency ('from') into another ('to')
    /// using standard D&D 5e conversion rates. Uses float calculation and floors the result.
    /// </summary>
    /// <param name="characterId">The unique identifier of the character.</param>
    /// <param name="from">The currency type to exchange from.</param>
    /// <param name="to">The currency type to exchange to.</param>
    /// <param name="fromAmount">The amount of 'from' currency to exchange.</param>
    /// <returns>The character with updated currency amounts.</returns>
    /// <exception cref="CurrencyNotInitializedException">...</exception>
    /// <exception cref="NotEnoughCurrencyException">Thrown when the character doesn't have enough 'from' currency.</exception>
    /// <exception cref="InvalidCurrencyExchangeException">Thrown for invalid exchanges (e.g., same type).</exception>
    /// <exception cref="OverflowException">Thrown if the resulting 'to' balance exceeds integer limits.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if amount is negative.</exception>
    public async Task<Character> ExchangeCurrencyAsync(
      Guid characterId,
      CurrencyType from,
      CurrencyType to,
      int fromAmount) {
      var character = await repository.GetByIdOrThrowAsync(characterId);
      if (!character.InitFlags.HasFlag(CharacterInitializationFlags.CurrencyInitialized)) {
        throw new CurrencyNotInitializedException(characterId);
      }

      if (from == to) {
        throw new InvalidCurrencyExchangeException(from, to, fromAmount);
      }

      if (fromAmount <= 0) {
        throw new InvalidCurrencyExchangeException(from, to, fromAmount);
      }

      // Check if character has enough 'from' currency
      var currentAmountFrom = character.Wealth.GetCurrencyAmount(from);
      if (currentAmountFrom < fromAmount) {
        throw new NotEnoughCurrencyException(from, fromAmount, currentAmountFrom);
      }

      // Calculate exchange rate using float
      float exchangeRate = GetExchangeRate(from, to);
      // Calculate the theoretical amount to add using float
      float floatAmountToAdd = fromAmount * exchangeRate;

      // Check for overflow before adding to the wealth
      // e.g: something close to overflow could be overflown after 10xing it
      if (floatAmountToAdd > int.MaxValue) {
        throw new OverflowException($"Intermediate exchange result ({floatAmountToAdd} {to}) exceeds integer limits.");
      }

      // Floor the float amount to get the integer amount to add
      int amountToAdd = (int) Math.Floor(floatAmountToAdd);

      // If we don't even have a unit to exchange, throw an invalid exchange exception
      if (amountToAdd == 0) {
        throw new InvalidCurrencyExchangeException(from, to, fromAmount);
      }

      // Verify that the amount to add + current amount of 'to' does not exceed integer limits
      var currentAmountTo = character.Wealth.GetCurrencyAmount(to);
      if (int.MaxValue - amountToAdd < currentAmountTo) {
        throw new OverflowException($"Resulting amount of {to} exceeds integer limits.");
      }

      // Apply changes: Subtract 'amount' of 'from', Add 'amountToAdd' of 'to'
      character.Wealth.SetCurrencyAmount(from, currentAmountFrom - fromAmount);
      character.Wealth.SetCurrencyAmount(to, currentAmountTo + amountToAdd);

      await repository.UpdateAsync(character);
      return character;
    }

    /// <summary>
    /// Gets the exchange rate multiplier between two currency types using float.
    /// Rate indicates how many 'to' units are obtained for one 'from' unit.
    /// </summary>
    private static float GetExchangeRate(CurrencyType from, CurrencyType to) {
      float fromCpValue = GetCpValue(from);
      float toCpValue = GetCpValue(to);
      return fromCpValue / toCpValue;
    }

    /// <summary>
    /// Gets the Copper Piece (CP) value for a given currency type.
    /// </summary>
    private static int GetCpValue(CurrencyType currency) {
      return currency switch {
        CurrencyType.Copper => 1,
        CurrencyType.Silver => 10,
        CurrencyType.Electrum => 50, // Assuming standard 5e rates
        CurrencyType.Gold => 100,
        CurrencyType.Platinum => 1000,
        _ => throw new ArgumentOutOfRangeException(nameof(currency),
                                                   $"Invalid or unsupported currency type specified: {currency}")
      };
    }
  }
}
