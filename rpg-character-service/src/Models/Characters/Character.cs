namespace RPGCharacterService.Models.Characters {
  public class Character {
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; set; }
    public string Race { get; set; }
    public string Subrace { get; set; }
    public string Class { get; set; }
    public int HitPoints { get; set; }
    public int Level { get; set; }
    public CharacterInitializationFlags InitFlags { get; set; } = 0;
    public Dictionary<AbilityScore, int> AbilityScores { get; init; } = new();
    public EquippedItems EquippedItems { get; init; } = new();
    public Wealth Wealth { get; set; } = new();
  }

  public class EquippedItems {
    public int? MainHand { get; set; }
    public int? OffHand { get; set; }
    public int? Armor { get; set; }
  }
}
