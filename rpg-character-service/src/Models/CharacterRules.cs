using RPGCharacterService.Models.Items;

namespace RPGCharacterService.Models
{
    public record CharacterDerivedProperties
    {
        public int ArmorClass { get; init; }
        public int ProficiencyBonus { get; init; }
    }
    
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
            var armorClassAdjuster = DndFifthEditionArmorClassAdjusterFactory.GetArmorClassAdjuster(armorType);
            var dexterityModifier = character.GetAbilityModifier(StatType.Dexterity);

            return armorClassAdjuster.AdjustArmorClass(baseArmorClass, dexterityModifier);
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
