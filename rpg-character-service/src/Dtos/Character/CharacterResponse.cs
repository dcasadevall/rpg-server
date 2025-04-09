using RPGCharacterService.Entities;
using RPGCharacterService.Entities.Characters;
using Swashbuckle.AspNetCore.Annotations;

namespace RPGCharacterService.Dtos.Character {
  /// <summary>
  ///   Response object for returning character information to the client.
  /// </summary>
  public record CharacterResponse {
    /// <summary>
    ///   The unique identifier of the character
    /// </summary>
    [SwaggerSchema(Description = "The unique identifier of the character")]
    public Guid Id { get; init; }

    /// <summary>
    ///   The name of the character
    /// </summary>
    [SwaggerSchema(Description = "The name of the character")]
    public required string Name { get; init; }

    /// <summary>
    ///   The character's race
    /// </summary>
    [SwaggerSchema(Description = "The character's race (e.g., Human, Elf, Dwarf)")]
    public required string Race { get; init; }

    /// <summary>
    ///   The character's subrace, if any
    /// </summary>
    [SwaggerSchema(Description = "The character's subrace, if applicable (e.g., High Elf, Mountain Dwarf)")]
    public required string Subrace { get; init; }

    /// <summary>
    ///   The character's class
    /// </summary>
    [SwaggerSchema(Description = "The character's class (e.g., Fighter, Wizard, Cleric)")]
    public required string Class { get; init; }

    /// <summary>
    /// The character's level
    /// </summary>
    [SwaggerSchema(Description = "The character's level")]
    public int Level { get; init; }

    /// <summary>
    ///   The character's current hit points
    /// </summary>
    [SwaggerSchema(Description = "The character's current hit points")]
    public int HitPoints { get; init; }

    /// <summary>
    ///   The character's ability scores
    /// </summary>
    [SwaggerSchema(Description = "The character's ability scores (Strength, Dexterity, etc.)")]
    public Dictionary<AbilityScore, int> AbilityScores { get; init; } = new();

    /// <summary>
    ///   The character's equipped items
    /// </summary>
    [SwaggerSchema(Description = "The character's currently equipped items")]
    public Entities.Equipment Equipment { get; init; } = new();

    /// <summary>
    ///   The character's wealth (currencies)
    /// </summary>
    [SwaggerSchema(Description = "The character's wealth in various currencies")]
    public Wealth Wealth { get; init; } = new();

    // Derived Properties

    /// <summary>
    ///   The character's maximum hit points
    /// </summary>
    [SwaggerSchema(Description = "The character's maximum hit points")]
    public int MaxHitPoints { get; init; }

    /// <summary>
    ///   The character's armor class
    /// </summary>
    [SwaggerSchema(Description = "The character's armor class")]
    public int ArmorClass { get; init; }

    /// <summary>
    ///   The character's proficiency bonus based on level
    /// </summary>
    [SwaggerSchema(Description = "The character's proficiency bonus based on level")]
    public int ProficiencyBonus { get; init; }

    /// <summary>
    ///   The character's ability modifiers derived from ability scores
    /// </summary>
    [SwaggerSchema(Description = "The character's ability modifiers derived from ability scores")]
    public Dictionary<AbilityScore, int> AbilityModifiers { get; init; } = new();
  }
}
