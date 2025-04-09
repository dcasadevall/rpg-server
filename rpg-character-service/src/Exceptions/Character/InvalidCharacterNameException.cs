namespace RPGCharacterService.Exceptions.Character {
  /// <summary>
  /// Exception thrown when a character name is invalid.
  /// Invalid names are either not the right length or contain invalid characters.
  /// </summary>
  /// <param name="msg"></param>
  public class InvalidCharacterNameException(string msg) : Exception(msg);
}
