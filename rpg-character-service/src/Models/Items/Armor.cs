namespace RPGCharacterService.Models.Items
{
    public record Armor
    {
        public ushort Id { get; init; }
        public int BaseArmorClass { get; init; }
        public ArmorType ArmorType { get; init; }
        public IArmorClassAdjuster ArmorClassAdjuster { get; init; }
    }
    
    public enum ArmorType
    {
        Light,
        Medium,
        Heavy,
        Shield,
    }
}
