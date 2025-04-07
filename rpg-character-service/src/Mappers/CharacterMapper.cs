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
} 
