using RPGCharacterService.Models.Items;

namespace RPGCharacterService.Models.Characters
{
    public class Character
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Race { get; set; }
        public string Subrace { get; set; }
        public string Class { get; set; }
        public int HitPoints { get; set; }
        public int Level { get; set; }
        public CharacterInitializationFlags InitFlags { get; set; } = 0;
        public Dictionary<StatType, int> Stats { get; init; } = new();
        public EquippedItems EquippedItems { get; init; } = new();
        public Wealth Wealth { get; set; } = new();
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
