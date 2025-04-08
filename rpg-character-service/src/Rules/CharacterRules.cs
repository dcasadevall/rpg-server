using RPGCharacterService.Models.Characters;

namespace RPGCharacterService.Rules {
  public interface ICharacterRules {
    int CalculateMaxHitPoints(Character character);
    int CalculateProficiencyBonus(Character character);
    Dictionary<AbilityScore, int> CalculateAbilityModifiers(Character character);
  }

  public class DndFifthEditionCharacterRules : ICharacterRules {
    public int CalculateProficiencyBonus(Character character) {
      return character.Level switch {
        >= 17 => 6,
        >= 13 => 5,
        >= 9 => 4,
        >= 5 => 3,
        _ => 2
      };
    }

    public Dictionary<AbilityScore, int> CalculateAbilityModifiers(Character character) {
      var modifiers = new Dictionary<AbilityScore, int>();
      foreach (var abilityScore in character.AbilityScores) {
        modifiers[abilityScore.Key] = (abilityScore.Value - 10) / 2;
      }

      return modifiers;
    }

    public int CalculateMaxHitPoints(Character character) {
      var abilityModifiers = CalculateAbilityModifiers(character);
      var constitutionModifier = abilityModifiers[AbilityScore.Constitution];
      return 10 + constitutionModifier * character.Level;
    }
  }
}
