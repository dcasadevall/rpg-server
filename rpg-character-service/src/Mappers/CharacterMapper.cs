using RPGCharacterService.Dtos.Character.Responses;
using RPGCharacterService.Models;

namespace RPGCharacterService.Mappers
{
    public static class CharacterMapper
    {
        public static CharacterResponse ToResponse(Character character)
        {
            return new CharacterResponse
            {
                Id = character.Id,
                Name = character.Name,
                Race = character.Race,
                Subrace = character.Subrace,
                Class = character.Class,
                Health = character.HitPoints,
                MaxHealth = character.MaxHitPoints,
                Stats = character.Stats,
                Currencies = character.Currencies,
                // Derived properties
                ProficiencyBonus = CalculateProficiencyBonus(character),
                ArmorClass = CalculateArmorClass(character),
            };
        }
        
        private static int CalculateProficiencyBonus(Character character)
        {
            // TODO: Calculate proficiency bonus based on character's level
            return 0;
        }

        private static int CalculateArmorClass(Character character)
        {
            // TODO: Calculate armor class based on character's equipment and stats
            return 0;
        }
    }
} 
