using RPGCharacterService.Models.Items;

namespace RPGCharacterService.Exceptions.Equipment {
  /// <summary>
  /// Exception thrown when attempting to equip an item in a slot that doesn't match its type.
  /// For example, trying to equip a weapon in the armor slot.
  /// </summary>
  public class EquipmentTypeMismatchException(int itemId, EquipmentType expectedType)
    : Exception($"Equipment with ID {itemId} is not of the expected type {expectedType}.");
}
