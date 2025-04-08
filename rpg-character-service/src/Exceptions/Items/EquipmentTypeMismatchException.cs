using RPGCharacterService.Models.Items;

namespace RPGCharacterService.Exceptions.Items {
  public class EquipmentTypeMismatchException(int itemId, EquipmentType expectedType)
    : Exception($"Equipment with ID {itemId} is not of the expected type {expectedType}.");
}
