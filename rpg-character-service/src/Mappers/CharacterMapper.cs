using RPGCharacterService.Dtos.Character;
using RPGCharacterService.Models.Characters;

namespace RPGCharacterService.Mappers {
  public static class CharacterMapper {
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
