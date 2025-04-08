namespace RPGCharacterService.Exceptions.Currency {
  /// <summary>
  /// Exception thrown when attempting to perform currency operations on a character that hasn't had their currency initialized.
  /// Currency must be initialized before any currency-related operations can be performed.
  /// </summary>
  public class CurrencyNotInitializedExceptions : Exception {
    public CurrencyNotInitializedExceptions() : base("Currency not initialized.") { }
  }
}
