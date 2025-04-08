namespace RPGCharacterService.Exceptions.Character {
  /// <summary>
  /// Exception thrown when attempting to perform an operation on a character that does not exist.
  /// </summary>
  public class CharacterNotFoundException(Guid characterId) : Exception($"Character with ID {characterId} not found.");
}
