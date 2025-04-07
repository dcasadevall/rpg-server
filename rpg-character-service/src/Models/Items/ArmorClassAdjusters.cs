namespace RPGCharacterService.Models.Items
{
    public interface IArmorClassAdjuster
    {
        int AdjustArmorClass(int armorClass, Armor armor, Character character);
    }

    public class LightArmorAdjuster : IArmorClassAdjuster
    {
        public int AdjustArmorClass(int armorClass, Armor armor, Character character)
        {
            var dexMod = character.GetAbilityModifier(StatType.Dexterity);
            return armorClass + armor.BaseArmorClass + dexMod;
        }
    }

    public class MediumArmorAdjuster : IArmorClassAdjuster
    {
        public int AdjustArmorClass(int armorClass, Armor armor, Character character)
        {
            // Medium caps Dex at +2
            var dexMod = character.GetAbilityModifier(StatType.Dexterity);
            return armorClass + armor.BaseArmorClass + Math.Min(dexMod, 2); 
        }
    }

    public class HeavyArmorAdjuster : IArmorClassAdjuster
    {
        public int AdjustArmorClass(int armorClass, Armor armor, Character character)
        {
            // Heavy armor ignores Dex
            return armorClass + armor.BaseArmorClass; 
        }
    }
    
    public class ShieldArmorAdjuster : IArmorClassAdjuster
    {
        public int AdjustArmorClass(int armorClass, Armor armor, Character character)
        {
            // Shield adds +2 to AC
            return armorClass + 2; 
        }
    }
}
