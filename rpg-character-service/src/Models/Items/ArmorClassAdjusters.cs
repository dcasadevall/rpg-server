namespace RPGCharacterService.Models.Items
{
    public interface IArmorClassAdjuster
    {
        int AdjustArmorClass(int baseArmorClass, int dexterityModifier);
    }

    public class LightArmorAdjuster : IArmorClassAdjuster
    {
        public int AdjustArmorClass(int baseArmorClass, int dexterityModifier)
        {
            return baseArmorClass + dexterityModifier;
        }
    }

    public class MediumArmorAdjuster : IArmorClassAdjuster
    {
        public int AdjustArmorClass(int baseArmorClass, int dexterityModifier)
        {
            return baseArmorClass + Math.Min(dexterityModifier, 2); 
        }
    }

    public class HeavyArmorAdjuster : IArmorClassAdjuster
    {
        public int AdjustArmorClass(int baseArmorClass, int dexterityModifier)
        {
            return baseArmorClass; 
        }
    }
    
    public class WithoutArmorClassAdjuster : IArmorClassAdjuster
    {
        public int AdjustArmorClass(int baseArmorClass, int dexterityModifier)
        {
            return 10 + dexterityModifier; 
        }
    }
    
    public static class DndFifthEditionArmorClassAdjusterFactory
    {
        private static readonly Dictionary<ArmorType, IArmorClassAdjuster> adjusters = new()
        {
            { ArmorType.Light, new LightArmorAdjuster() },
            { ArmorType.Medium, new MediumArmorAdjuster() },
            { ArmorType.Heavy, new HeavyArmorAdjuster() },
            { ArmorType.None, new WithoutArmorClassAdjuster() }
        };

        public static IArmorClassAdjuster GetArmorClassAdjuster(ArmorType armorType)
        {
            if (!adjusters.TryGetValue(armorType, out var calculator))
            {
                throw new NotSupportedException($"No calculator for armor type {armorType}");
            }

            return calculator;
        }
    }
}
