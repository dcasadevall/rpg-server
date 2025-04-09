using RPGCharacterService.Exceptions.Character;

namespace RPGCharacterService.Services {
  public interface ICharacterValidator {
    /// <summary>
    /// Validates that all the given character details conform to the rules specified in the API documentation.
    /// </summary>
    /// <param name="name">The name of the character</param>
    /// <param name="race">The race of the character</param>
    /// <param name="subrace">The optional subrace</param>
    /// <param name="characterClass">The chosen class</param>
    /// <exception cref="InvalidCharacterNameException">Thrown when the name format is invalid.</exception>
    /// <exception cref="InappropriateNameException">Thrown when the name is inappropriate.</exception>
    /// <exception cref="InvalidRaceException">Thrown when the race is invalid.</exception>
    /// <exception cref="InvalidCharacterClassException">Thrown when the class is invalid.</exception>
    public void ValidateCharacterDetails(string name, string race, string? subrace, string characterClass);
  }

  /// <summary>
  /// Implementation of ICharacterValidator that uses a set of hardcoded race / subrace and classes,
  /// as well as inappropriate names
  /// </summary>
  public class HardcodedCharacterValidator : ICharacterValidator {
    private static readonly HashSet<string> INAPPROPRIATE_NAMES = new(StringComparer.OrdinalIgnoreCase) {
      "admin", "moderator", "gamemaster", "dungeonmaster",
    };

    // Valid race options based on API documentation
    private static readonly HashSet<string> VALID_RACES = new(StringComparer.OrdinalIgnoreCase) {
      "Dwarf", "Elf", "Halfling", "Human", "Dragonborn", "Gnome", "Half-Elf", "Half-Orc", "Tiefling"
    };

    // Valid subraces per race based on API documentation
    private static readonly Dictionary<string, HashSet<string>> VALID_SUBRACES = new(StringComparer.OrdinalIgnoreCase) {
      {"Dwarf", new HashSet<string>(StringComparer.OrdinalIgnoreCase) {"Hill", "Mountain"}},
      {"Elf", new HashSet<string>(StringComparer.OrdinalIgnoreCase) {"High", "Wood", "Drow"}},
      {"Halfling", new HashSet<string>(StringComparer.OrdinalIgnoreCase) {"Lightfoot", "Stout"}},
      {"Gnome", new HashSet<string>(StringComparer.OrdinalIgnoreCase) {"Forest", "Rock", "Deep"}}
    };

    // Valid character classes based on API documentation
    private static readonly HashSet<string> VALID_CLASSES = new(StringComparer.OrdinalIgnoreCase) {
      "Cleric", "Fighter", "Rogue", "Wizard", "Barbarian", "Bard", "Druid", "Monk", "Paladin", "Ranger", "Sorcerer",
      "Warlock"
    };

    /// <inheritdoc />
    public void ValidateCharacterDetails(string name, string race, string? subrace, string characterClass) {
      ValidateName(name);
      ValidateClass(characterClass);
      ValidateRaceAndSubrace(race, subrace);
    }

    /// <summary>
    /// Validates the character name according to the rules specified in the API documentation.
    /// </summary>
    /// <param name="name">The name to validate.</param>
    /// <exception cref="InvalidCharacterNameException">Thrown when the name format is invalid.</exception>
    /// <exception cref="InappropriateNameException">Thrown when the name is inappropriate.</exception>
    private void ValidateName(string name) {
      // Check if name is null or empty
      if (string.IsNullOrWhiteSpace(name)) {
        throw new InvalidCharacterNameException("Name cannot be empty.");
      }

      // Check name length (3-15 characters)
      if (name.Length < 3 || name.Length > 15) {
        throw new InvalidCharacterNameException("Name must be between 3 and 15 characters long.");
      }

      // Check if name contains only letters
      if (!name.All(char.IsLetter)) {
        throw new InvalidCharacterNameException("Name must contain only letters (A-Z, a-z).");
      }

      // Check for inappropriate names
      if (INAPPROPRIATE_NAMES.Contains(name)) {
        throw new InappropriateNameException(name);
      }
    }

    /// <summary>
    /// Validates the character race and subrace according to the rules specified in the API documentation.
    /// </summary>
    /// <param name="race">The race to validate.</param>
    /// <param name="subrace">The subrace to validate.</param>
    /// <exception cref="InvalidRaceException">Thrown when the race is invalid.</exception>
    private void ValidateRaceAndSubrace(string race, string? subrace) {
      if (string.IsNullOrWhiteSpace(race)) {
        throw new InvalidRaceException("Race is required.");
      }

      if (!VALID_RACES.Contains(race)) {
        throw new InvalidRaceException($"Race must be one of: {string.Join(", ", VALID_RACES)}.");
      }

      // Validate subrace if provided
      if (!string.IsNullOrWhiteSpace(subrace)) {
        // Check if the race has subraces
        if (!VALID_SUBRACES.ContainsKey(race)) {
          throw new InvalidRaceException($"The race '{race}' does not have subraces.");
        }

        // Check if the subrace is valid for the race
        if (!VALID_SUBRACES[race]
          .Contains(subrace)) {
          throw new
            InvalidRaceException($"Subrace must be one of: {string.Join(", ", VALID_SUBRACES[race])} for race '{race}'.");
        }
      }
    }

    /// <summary>
    /// Validates the character class according to the rules specified in the API documentation.
    /// </summary>
    /// <param name="characterClass">The class to validate.</param>
    /// <exception cref="InvalidCharacterClassException">Thrown when the class is invalid.</exception>
    private void ValidateClass(string characterClass) {
      if (string.IsNullOrWhiteSpace(characterClass)) {
        throw new InvalidCharacterClassException("Class is required.");
      }

      if (!VALID_CLASSES.Contains(characterClass)) {
        throw new InvalidCharacterClassException($"Class must be one of: {string.Join(", ", VALID_CLASSES)}.");
      }
    }
  }
}
