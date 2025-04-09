namespace RPGCharacterService.Exceptions.Character {
  /// <summary>
  /// Exception thrown when a character class is invalid.
  /// </summary>
  /// <param name="msg"></param>
  public class InvalidCharacterClassException(string msg) : Exception(msg);
}
