using RPGCharacterService.Dtos.Character.Responses;
using RPGCharacterService.Models.Characters;

namespace RPGCharacterService.Mappers {
  public static class CharacterMapper {
    public static CharacterResponse ToResponse(Character character,
                                               CharacterDerivedProperties derivedProperties) {
      return new CharacterResponse {
        Id = character.Id,
        Name = character.Name,
        Race = character.Race,
        Subrace = character.Subrace,
        Class = character.Class,
        HitPoints = character.HitPoints,
        AbilityScores = character.AbilityScores,
        Wealth = character.Wealth,

        // Derived Properties
        MaxHitPoints = character.HitPoints,
        ArmorClass = derivedProperties.ArmorClass,
        ProficiencyBonus = derivedProperties.ProficiencyBonus,
        AbilityModifiers = derivedProperties.AbilityModifiers
      };
    }
  }
}
