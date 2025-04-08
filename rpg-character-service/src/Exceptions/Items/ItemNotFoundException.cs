namespace RPGCharacterService.Exceptions.Items {
  /// <summary>
  /// Exception thrown when a requested item cannot be found in the system.
  /// </summary>
  public class ItemNotFoundException : Exception {
    public ItemNotFoundException() : base("Item not found.") { }
  }
}
