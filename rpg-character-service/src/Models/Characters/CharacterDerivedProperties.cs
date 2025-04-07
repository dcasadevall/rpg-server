namespace RPGCharacterService.Models.Characters
{
    public record CharacterDerivedProperties
    {
        public int ArmorClass { get; init; }
        public int ProficiencyBonus { get; init; }
    }
}
