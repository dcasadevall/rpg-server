using RPGCharacterService.Models.Items;

namespace RPGCharacterService.Models.Characters
{
    public interface ICharacterRules
    {
        int CalculateMaxHitPoints(Character character);
        int CalculateArmorClass(Character character);
        int CalculateProficiencyBonus(Character character);
        Dictionary<AbilityScore, int> CalculateAbilityModifiers(Character character);
    }

    public class DndFifthEditionCharacterRules : ICharacterRules
    {
        public int CalculateArmorClass(Character character)
        {
            var abilityModifiers = CalculateAbilityModifiers(character);
            var armorType = character.EquippedItems.ArmorType;
            var baseArmorClass = character.EquippedItems.BaseArmorClass;
            var dexterityModifier = abilityModifiers[AbilityScore.Dexterity];

            return armorType switch
            {
                ArmorType.Light => baseArmorClass + dexterityModifier,
                ArmorType.Medium => baseArmorClass + Math.Min(dexterityModifier, 2),
                ArmorType.Heavy => baseArmorClass,
                ArmorType.None => 10 + dexterityModifier,
                _ => throw new NotSupportedException($"Unknown armor type: {armorType}")
            };
        }

        public int CalculateProficiencyBonus(Character character)
        {
            if (character.Level >= 17) return 6;
            if (character.Level >= 13) return 5;
            if (character.Level >= 9) return 4;
            if (character.Level >= 5) return 3;
            return 2;
        }
        
        public Dictionary<AbilityScore, int> CalculateAbilityModifiers(Character character)
        {
            var modifiers = new Dictionary<AbilityScore, int>();
            foreach (var abilityScore in character.AbilityScores)
            {
                modifiers[abilityScore.Key] = (abilityScore.Value - 10) / 2;
            }
            
            return modifiers;
        }
        
        public int CalculateMaxHitPoints(Character character)
        {
            var abilityModifiers = CalculateAbilityModifiers(character);
            var constitutionModifier = abilityModifiers[AbilityScore.Constitution];
            return 10 + constitutionModifier * character.Level;
        }
    }
}
