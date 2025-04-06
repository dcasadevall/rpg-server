using RPGCharacterService.Models;

namespace RPGCharacterService.Dtos.Character.Responses
{
    public class CharacterResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Race { get; set; }
        public string Subrace { get; set; }
        public string Class { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int ArmorClass { get; set; }
        public int ProficiencyBonus { get; set; }
        public Equipment Equipment { get; set; } = new();
        public Dictionary<StatType, int> Stats { get; set; }
        public Dictionary<CurrencyType, int> Currencies { get; set; }
    }
}
