namespace RPGCharacterService.Models.Items
{
    public record Item
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        
        public EquipmentStats? EquipmentStats { get; init; }
    }
    
    public record EquipmentStats
    {
        public int ArmorBonus { get; init; }
        public int DamageBonus { get; init; }
        public EquipmentType EquipmentType { get; init; }
        public ArmorStats? ArmorStats { get; init; }
        public WeaponStats? WeaponStats { get; init; }
    }
    
    public record ArmorStats
    {
        public int BaseArmorClass { get; init; } = 0;
        public ArmorType ArmorType { get; init; } = ArmorType.Light;
    }
    
    public record WeaponStats
    {
        public int Damage { get; init; }
        public WeaponProperty WeaponProperties { get; init; } = WeaponProperty.None;
        public WeaponRangeType RangeType { get; init; } = WeaponRangeType.Melee;
        public WeaponCategory Category { get; init; } = WeaponCategory.Simple;
    }
}
