namespace RPGCharacterService.Models
{
    public class Character
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Race { get; set; }
        public string Subrace { get; set; }
        public string Class { get; set; }
        public int HitPoints { get; set; }
        public int MaxHitPoints { get; set; }
        public Dictionary<StatType, int> Stats { get; init; } = new();
        public Equipment Equipment { get; set; } = new();
        public Dictionary<CurrencyType, int> Currencies { get; set; }
    }

    public class Equipment
    {
        public int MainHand { get; set; }
        public int OffHand { get; set; }
        public int Shield { get; set; }
        public int Armor { get; set; }
    }
} 
