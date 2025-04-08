namespace RPGCharacterService.Exceptions.Character {
    public class InappropriateNameException(string name) : Exception($"The name '{name}' is inappropriate and cannot be used.");
}
