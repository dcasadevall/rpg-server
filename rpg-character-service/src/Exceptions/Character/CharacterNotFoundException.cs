namespace RPGCharacterService.Exceptions.Character {
  public class CharacterNotFoundException(Guid characterId) : Exception($"Character with ID {characterId} not found.");
}
