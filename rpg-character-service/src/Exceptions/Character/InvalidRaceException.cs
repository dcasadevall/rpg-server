namespace RPGCharacterService.Exceptions.Character {
  /// <summary>
  /// Exception thrown when an invalid race is provided for a character.
  /// </summary>
  /// <param name="msg"></param>
  public class InvalidRaceException(string msg) : Exception(msg);
}
