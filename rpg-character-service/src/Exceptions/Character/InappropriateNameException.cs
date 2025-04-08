namespace RPGCharacterService.Exceptions.Character {
    /// <summary>
    /// Exception thrown when attempting to create a character with an inappropriate name.
    /// Inappropriate names include reserved terms like 'admin', 'moderator', etc.
    /// </summary>
    public class InappropriateNameException(string name) : Exception($"The name '{name}' is inappropriate and cannot be used.");
}
