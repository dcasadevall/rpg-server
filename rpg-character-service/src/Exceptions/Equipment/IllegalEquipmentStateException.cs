using RPGCharacterService.Entities.Items;

namespace RPGCharacterService.Exceptions.Equipment {
  /// <summary>
  /// A generic exception for illegal equipment states.
  /// Examples: Two-handed weapon equipped in the off hand.
  /// </summary>
  /// <param name="msg"></param>
  public class IllegalEquipmentStateException(string msg) : Exception(msg);
}
