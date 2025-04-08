namespace RPGCharacterService.Models.Equipment
{
    public class EquipmentItem
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public EquipmentType Type { get; set; }
        public int ArmorClass { get; set; }
        public bool IsTwoHanded { get; set; }
        public bool CanBeOffHand { get; set; }
    }
} 
