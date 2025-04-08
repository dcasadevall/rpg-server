namespace RPGCharacterService.Exceptions.Items {
  /// <summary>
  /// Exception thrown when a requested item cannot be found in the system.
  /// </summary>
  public class ItemNotFoundException(int itemId) : Exception($"Item with ID {itemId} not found.") {
  }
}
