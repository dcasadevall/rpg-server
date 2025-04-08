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
    public async Task<Character>
      ModifyCurrenciesAsync(Guid characterId, Dictionary<CurrencyType, int> currencyChanges) {
      var character = await repository.GetByIdOrThrowAsync(characterId);

      if (!character.InitFlags.HasFlag(CharacterInitializationFlags.CurrencyInitialized)) {
        throw new CurrencyNotInitializedException(characterId);
      }

      // Apply changes
      foreach (var change in currencyChanges) {
        var currencyAmountAfter = character.Wealth.GetCurrencyAmount(change.Key) + change.Value;
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
    /// Uses integer arithmetic based on direct Copper Piece value comparison to avoid floating-point errors.
    /// </summary>
    /// <param name="characterId">The unique identifier of the character.</param>
    /// <param name="from">The currency type to exchange from.</param>
    /// <param name="to">The currency type to exchange to.</param>
    /// <param name="amount">The amount of 'from' currency to exchange. Must result in a whole number of 'to' units.</param>
    /// <returns>The character with updated currency amounts after the exchange.</returns>
    /// <exception cref="CurrencyNotInitializedException">Thrown when currency has not been initialized for the character.</exception>
    /// <exception cref="NotEnoughCurrencyException">Thrown when the character doesn't have enough currency for the exchange.</exception>
    /// <exception cref="InvalidCurrencyExchangeException">Thrown when attempting an exchange between the same currency type or unsupported types.</exception>
    /// <exception cref="InvalidExchangeAmountException">Thrown when the amount is negative or does not result in a whole number of 'to' units.</exception>
    /// <exception cref="OverflowException">Thrown when the exchange would result in an overflow for the target currency.</exception>
    public async Task<Character>
      ExchangeCurrencyAsync(Guid characterId, CurrencyType from, CurrencyType to, int amount) {
      var character = await repository.GetByIdOrThrowAsync(characterId);
      if (!character.InitFlags.HasFlag(CharacterInitializationFlags.CurrencyInitialized)) {
        throw new CurrencyNotInitializedException(characterId);
      }

      // Prevent exchanging a currency for itself or invalid amounts
      if (from == to) {
        throw new InvalidCurrencyExchangeException(from, to, amount);
      }

      if (amount <= 0) {
        throw new InvalidExchangeAmountException(from, to, amount);
      }

      // Check if character has enough currency to exchange
      var currentAmountFrom = character.Wealth.GetCurrencyAmount(from);
      if (currentAmountFrom < amount) {
        throw new NotEnoughCurrencyException(from, amount, currentAmountFrom);
      }

      // Get Copper Piece values
      var fromCpValue = GetCpValue(from);
      var toCpValue = GetCpValue(to);

      // Calculate total offered CP
      // Use long to prevent potential overflow if amount * fromCpValue exceeds int.MaxValue
      var totalOfferedCp = (long) amount * fromCpValue;

      // Validate that the offered CP is perfectly divisible by the target CP value
      if (totalOfferedCp % toCpValue != 0) {
        // Use the simpler exception constructor
        throw new InvalidExchangeAmountException(from, to, amount);
      }

      // Calculate integer amounts
      var amountToAdd = (int) (totalOfferedCp / toCpValue);
      var currentAmountTo = character.Wealth.GetCurrencyAmount(to);

      // Check for overflow
      if (currentAmountTo > int.MaxValue - amountToAdd) {
        throw new OverflowException("The exchange would result in an overflow for the target currency.");
      }

      // Update currency amounts using integers
      character.Wealth.SetCurrencyAmount(from, currentAmountFrom - amount);
      character.Wealth.SetCurrencyAmount(to, currentAmountTo + amountToAdd);

      await repository.UpdateAsync(character);
      return character;
    }

    /// <summary>
    /// Gets the Copper Piece (CP) value for a given currency type.
    /// </summary>
    private static int GetCpValue(CurrencyType currency) {
      return currency switch {
        CurrencyType.Copper => 1,
        CurrencyType.Silver => 10,
        CurrencyType.Electrum => 50,
        CurrencyType.Gold => 100,
        CurrencyType.Platinum => 1000,
        _ => throw new ArgumentOutOfRangeException(nameof(currency),
                                                   $"Invalid currency type: {currency}") // Should be unreachable with enum
      };
    }
  }

  /// <summary>
  /// Exception class for invalid exchange amounts.
  /// </summary>
  public class InvalidExchangeAmountException : Exception {
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidExchangeAmountException"/> class.
    /// </summary>
    /// <param name="from">The currency type to exchange from.</param>
    /// <param name="to">The currency type to exchange to.</param>
    /// <param name="amount">The amount of currency to exchange.</param>
    public InvalidExchangeAmountException(CurrencyType from, CurrencyType to, int amount) :
      base($"Invalid amount {amount} specified for exchange from {from} to {to}. Amount does not result in a whole number of {to} units.") {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidExchangeAmountException"/> class.
    /// </summary>
    /// <param name="from">The currency type to exchange from.</param>
    /// <param name="to">The currency type to exchange to.</param>
    /// <param name="amount">The amount of currency to exchange.</param>
    /// <param name="requiredMultiple">The required multiple for the exchange.</param>
    public InvalidExchangeAmountException(CurrencyType from, CurrencyType to, int amount, int requiredMultiple) :
      base($"Invalid amount {amount} specified for exchange from {from} to {to}. Amount must be a non-negative multiple of {requiredMultiple}.") {
    }
  }
}
