using RPGCharacterService.Models.Items;

namespace RPGCharacterService.Models
{
    public class Character
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Race { get; set; }
        public string Subrace { get; set; }
        public string Class { get; set; }
        public int HitPoints { get; set; }
        public int MaxHitPoints { get; set; }
        public int Level { get; set; }
        public Dictionary<StatType, int> Stats { get; init; } = new();
        public EquippedItems EquippedItems { get; set; } = new();
        public Dictionary<CurrencyType, int> Currencies { get; set; }
        
        public int ProficiencyBonus
        {
            get
            {
                if (Level >= 17) return 6;
                if (Level >= 13) return 5;
                if (Level >= 9) return 4;
                if (Level >= 5) return 3;
                return 2;
            }
        }
        
        public int GetAbilityModifier(StatType stat)
        {
            if (Stats.TryGetValue(stat, out var score))
            {
                return (score - 10) / 2;
            }
            
            return 0;
        }
    }

    public class EquippedItems
    {
        public Item? MainHand { get; set; }
        public Item? OffHand { get; set; }
        public Item? Armor { get; set; }
        
        public ArmorType ArmorType => Armor?.EquipmentStats?.ArmorStats?.ArmorType ?? ArmorType.None;
        public int BaseArmorClass => Armor?.EquipmentStats?.ArmorStats?.BaseArmorClass ?? 0;
    }
} 
