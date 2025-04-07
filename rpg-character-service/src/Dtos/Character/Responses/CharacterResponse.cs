using RPGCharacterService.Models;
using RPGCharacterService.Models.Characters;

namespace RPGCharacterService.Dtos.Character.Responses
{
    public record CharacterResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Race { get; init; }
        public string Subrace { get; init; }
        public string Class { get; init; }
        public int HitPoints { get; init; }
        public EquippedItems Equipment { get; init; } = new();
        public Dictionary<StatType, int> Stats { get; init; } = new();
        public Wealth Wealth { get; init; } = new();
        
        // Derived Properties
        public int MaxHitPoints { get; init; }
        public int ArmorClass { get; init; }
        public int ProficiencyBonus { get; init; }
        public Dictionary<StatType, int> AbilityModifiers { get; init; } = new();
    }
}
