namespace RPGCharacterService.Exceptions.Character {
  /// <summary>
  /// Exception thrown when attempting to create a character with a name that is already in use.
  /// </summary>
  public class CharacterAlreadyExistsException(string characterName)
    : Exception($"Character with name {characterName} already exists.");
}
