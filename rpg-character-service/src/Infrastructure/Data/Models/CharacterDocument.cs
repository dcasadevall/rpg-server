using Amazon.DynamoDBv2.DataModel;

namespace RPGCharacterService.Infrastructure.Data.Models {
  /// <summary>
  /// DynamoDB representation of a character.
  /// </summary>
  [DynamoDBTable("characters")]
  public class CharacterDocument {
    [DynamoDBHashKey]
    public string Id { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string Name { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string Race { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string Subrace { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string Class { get; set; } = string.Empty;

    [DynamoDBProperty]
    public int Level { get; set; }

    [DynamoDBProperty]
    public int HitPoints { get; set; }

    [DynamoDBProperty]
    public Dictionary<string, int> AbilityScores { get; set; } = new();

    [DynamoDBProperty]
    public EquipmentDocument Equipment { get; set; } = new();

    [DynamoDBProperty]
    public WealthDocument Wealth { get; set; } = new();
  }
  /// <summary>
  /// DynamoDB representation of a character's equipment.
  /// </summary>
  public class EquipmentDocument {
    [DynamoDBProperty]
    public ItemDocument? MainHand { get; set; }

    [DynamoDBProperty]
    public ItemDocument? OffHand { get; set; }

    [DynamoDBProperty]
    public ItemDocument? Armor { get; set; }
  }

  /// <summary>
  /// DynamoDB representation of a character's wealth.
  /// </summary>
  public class WealthDocument {
    [DynamoDBProperty]
    public int Copper { get; set; }

    [DynamoDBProperty]
    public int Silver { get; set; }

    [DynamoDBProperty]
    public int Electrum { get; set; }

    [DynamoDBProperty]
    public int Gold { get; set; }

    [DynamoDBProperty]
    public int Platinum { get; set; }
  }
}
