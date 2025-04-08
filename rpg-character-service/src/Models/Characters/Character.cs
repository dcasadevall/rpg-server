using RPGCharacterService.Models.Items;

namespace RPGCharacterService.Models.Characters {
  public class Character {
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; init; }
    public string Race { get; init; }
    public string Subrace { get; init; }
    public string Class { get; init; }
    public int HitPoints { get; set; }
    public int Level { get; set; }
    public CharacterInitializationFlags InitFlags { get; set; } = 0;
    public Dictionary<AbilityScore, int> AbilityScores { get; init; } = new();
    public EquippedItems Equipment { get; init; } = new();
    public Wealth Wealth { get; set; } = new();
  }

  public class EquippedItems {
    public Item? MainHand { get; set; }
    public Item? OffHand { get; set; }
    public Item? Armor { get; set; }
  }
}
