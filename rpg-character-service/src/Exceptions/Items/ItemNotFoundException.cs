namespace RPGCharacterService.Exceptions.Items {
  public class ItemNotFoundException(int itemId) : Exception($"Item with ID {itemId} not found.") {
  }
}
