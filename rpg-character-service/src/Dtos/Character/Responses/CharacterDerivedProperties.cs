using RPGCharacterService.Models.Characters;

namespace RPGCharacterService.Dtos.Character.Responses {
  public record CharacterDerivedProperties {
    public int MaxHitPoints { get; init; }
    public int ArmorClass { get; init; }
    public int ProficiencyBonus { get; init; }
    public Dictionary<AbilityScore, int> AbilityModifiers { get; init; } = new();
  }
}
