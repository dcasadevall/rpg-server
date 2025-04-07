using RPGCharacterService.Models.Items;

namespace RPGCharacterService.Models.Characters
{
    public interface ICharacterRules
    {
        CharacterDerivedProperties GetDerivedProperties(Character character);
    }

    public class DndFifthEditionCharacterRules : ICharacterRules
    {
        public CharacterDerivedProperties GetDerivedProperties(Character character)
        {
            return new CharacterDerivedProperties
            {
                ArmorClass = CalculateArmorClass(character),
                ProficiencyBonus = CalculateProficiencyBonus(character)
            };
        }
        
        private int CalculateArmorClass(Character character)
        {
            var armorType = character.EquippedItems.ArmorType;
            var baseArmorClass = character.EquippedItems.BaseArmorClass;
            var dexterityModifier = character.GetAbilityModifier(StatType.Dexterity);

            return armorType switch
            {
                ArmorType.Light => baseArmorClass + dexterityModifier,
                ArmorType.Medium => baseArmorClass + Math.Min(dexterityModifier, 2),
                ArmorType.Heavy => baseArmorClass,
                ArmorType.None => 10 + dexterityModifier,
                _ => throw new NotSupportedException($"Unknown armor type: {armorType}")
            };
        }

        private int CalculateProficiencyBonus(Character character)
        {
            if (character.Level >= 17) return 6;
            if (character.Level >= 13) return 5;
            if (character.Level >= 9) return 4;
            if (character.Level >= 5) return 3;
            return 2;
        }
    }
}
