namespace RPGCharacterService.Exceptions.Character
{
    public class CharacterAlreadyExistsException(string characterName)
        : Exception($"Character with name {characterName} already exists.");
} 
