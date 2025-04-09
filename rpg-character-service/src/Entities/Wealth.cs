namespace RPGCharacterService.Entities {
  /// <summary>
  /// Represents the different types of currency.
  /// </summary>
  public enum CurrencyType {
    /// <summary>
    /// Copper pieces, the most basic currency.
    /// </summary>
    Copper,

    /// <summary>
    /// Silver pieces, worth 10 copper pieces.
    /// </summary>
    Silver,

    /// <summary>
    /// Electrum pieces, worth 5 silver pieces.
    /// </summary>
    Electrum,

    /// <summary>
    /// Gold pieces, worth 10 silver pieces.
    /// </summary>
    Gold,

    /// <summary>
    /// Platinum pieces, worth 10 gold pieces.
    /// </summary>
    Platinum
  }

  /// <summary>
  /// Represents a character's wealth in various currency types.
  /// This class manages the different currency amounts and provides methods to get and set them.
  /// </summary>
  public class Wealth {
    /// <summary>
    /// Gets or sets the amount of copper pieces.
    /// </summary>
    public int Copper { get; private set; }

    /// <summary>
    /// Gets or sets the amount of silver pieces.
    /// </summary>
    public int Silver { get; private set; }

    /// <summary>
    /// Gets or sets the amount of electrum pieces.
    /// </summary>
    public int Electrum { get; private set; }

    /// <summary>
    /// Gets or sets the amount of gold pieces.
    /// </summary>
    public int Gold { get; private set; }

    /// <summary>
    /// Gets or sets the amount of platinum pieces.
    /// </summary>
    public int Platinum { get; private set; }

    /// <summary>
    /// Default constructor for the Wealth class.
    /// </summary>
    public Wealth() {
    }

    /// <summary>
    /// Constructor for the Wealth class that initializes the currency amounts with
    /// the provided dictionary.
    /// </summary>
    /// <param name="currencyAmounts"></param>
    public Wealth(Dictionary<CurrencyType, int> currencyAmounts) {
      Copper = currencyAmounts.GetValueOrDefault(CurrencyType.Copper, 0);
      Silver = currencyAmounts.GetValueOrDefault(CurrencyType.Silver, 0);
      Electrum = currencyAmounts.GetValueOrDefault(CurrencyType.Electrum, 0);
      Gold = currencyAmounts.GetValueOrDefault(CurrencyType.Gold, 0);
      Platinum = currencyAmounts.GetValueOrDefault(CurrencyType.Platinum, 0);
    }

    /// <summary>
    /// Gets the amount of a specific currency type.
    /// </summary>
    /// <param name="currencyType">The type of currency to get the amount for.</param>
    /// <returns>The amount of the specified currency.</returns>
    /// <exception cref="ArgumentException">Thrown when an unknown currency type is provided.</exception>
    public int GetCurrencyAmount(CurrencyType currencyType) {
      return currencyType switch {
        CurrencyType.Copper => Copper,
        CurrencyType.Silver => Silver,
        CurrencyType.Electrum => Electrum,
        CurrencyType.Gold => Gold,
        CurrencyType.Platinum => Platinum,
        _ => throw new ArgumentException($"Unknown currency type: {currencyType}")
      };
    }

    /// <summary>
    /// Sets the amount of a specific currency type.
    /// </summary>
    /// <param name="currencyType">The type of currency to set the amount for.</param>
    /// <param name="amount">The new amount for the specified currency.</param>
    /// <exception cref="ArgumentException">Thrown when an unknown currency type is provided.</exception>
    public void SetCurrencyAmount(CurrencyType currencyType, int amount) {
      switch (currencyType) {
        case CurrencyType.Copper:
          Copper = amount;
          break;
        case CurrencyType.Silver:
          Silver = amount;
          break;
        case CurrencyType.Electrum:
          Electrum = amount;
          break;
        case CurrencyType.Gold:
          Gold = amount;
          break;
        case CurrencyType.Platinum:
          Platinum = amount;
          break;
        default:
          throw new ArgumentException($"Unknown currency type: {currencyType}");
      }
    }
  }
}
