using RPGCharacterService.Entities;
using RPGCharacterService.Entities.Characters;
using RPGCharacterService.Entities.Items;

namespace RPGCharacterService.Tests.Unit.Helpers {
  public static class CharacterFactory {
    public static Character CreateCharacter(string? name = null,
                                            string? race = null,
                                            string? subrace = null,
                                            string? characterClass = null,
                                            int? level = null,
                                            int? hitPoints = null,
                                            CharacterInitializationFlags initFlags = CharacterInitializationFlags.None,
                                            Dictionary<AbilityScore, int>? abilityScores = null,
                                            Equipment? equipment = null,
                                            Wealth? wealth = null) {

      return new Character {
        Name = name ?? "Test Character",
        Race = race ?? "Human",
        Subrace = subrace ?? string.Empty,
        Class = characterClass ?? "Fighter",
        Level = level ?? 1,
        HitPoints = hitPoints ?? 10,
        AbilityScores = abilityScores ??
                        new Dictionary<AbilityScore, int> {
                          {AbilityScore.Strength, 10},
                          {AbilityScore.Dexterity, 10},
                          {AbilityScore.Constitution, 10},
                          {AbilityScore.Intelligence, 10},
                          {AbilityScore.Wisdom, 10},
                          {AbilityScore.Charisma, 10}
                        },
        InitFlags = initFlags,
        Equipment = equipment ?? new Equipment(),
        Wealth = wealth ?? new Wealth()
      };
    }
  }
}
