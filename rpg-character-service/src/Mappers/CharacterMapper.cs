using RPGCharacterService.Dtos.Character;
using RPGCharacterService.Models.Characters;

namespace RPGCharacterService.Mappers {
  /// <summary>
  /// Provides mapping functionality between Character domain models and DTOs.
  /// This mapper handles the conversion of Character objects to their corresponding response DTOs,
  /// including the calculation of derived properties.
  /// </summary>
  public static class CharacterMapper {
    /// <summary>
    /// Converts a Character domain model to a CharacterResponse DTO.
    /// This includes mapping basic properties and calculating derived properties like max hit points,
    /// armor class, proficiency bonus, and ability modifiers.
    /// </summary>
    /// <param name="character">The Character domain model to convert.</param>
    /// <returns>A CharacterResponse DTO containing the mapped data.</returns>
    public static CharacterResponse ToResponse(Character character) {
      return new CharacterResponse {
        Id = character.Id,
        Name = character.Name,
        Race = character.Race,
        Subrace = character.Subrace,
        Class = character.Class,
        HitPoints = character.HitPoints,
        AbilityScores = character.AbilityScores,
        Wealth = character.Wealth,
        Equipment = character.Equipment,

        // Derived Properties
        MaxHitPoints = character.CalculateMaxHitPoints(),
        ArmorClass = character.CalculateArmorClass(),
        ProficiencyBonus = character.CalculateProficiencyBonus(),
        AbilityModifiers = character.CalculateAllAbilityModifiers()
      };
    }
  }
}
