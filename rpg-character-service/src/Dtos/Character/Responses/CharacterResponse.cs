using RPGCharacterService.Models;

namespace RPGCharacterService.Dtos.Character.Responses
{
    public record CharacterResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Race { get; init; }
        public string Subrace { get; init; }
        public string Class { get; init; }
        public int Health { get; init; }
        public int MaxHealth { get; init; }
        public EquippedItems Equipment { get; init; } = new();
        public Dictionary<StatType, int> Stats { get; init; }
        public Dictionary<CurrencyType, int> Currencies { get; init; }
        // Derived Properties
        public int ArmorClass { get; init; }
        public int ProficiencyBonus { get; init; }
    }
}
