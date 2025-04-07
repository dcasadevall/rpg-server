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
        public EquippedSlots EquippedSlots { get; set; } = new();
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
        
        public int ArmorClass => 10 + GetAbilityModifier(StatType.Dexterity);


        private static int CalculateArmorClass(Character character)
        {
            // TODO: Calculate armor class based on character's equipment and stats
            return 0;
        }
    }

    public class EquippedSlots
    {
        public int MainHand { get; set; }
        public int OffHand { get; set; }
        public int Armor { get; set; }
    }
    
    public enum StatType
    {
        Strength,
        Dexterity,
        Constitution,
        Intelligence,
        Wisdom,
        Charisma
    }
} 
