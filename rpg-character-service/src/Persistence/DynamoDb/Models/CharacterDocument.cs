using Amazon.DynamoDBv2.DataModel;
using RPGCharacterService.Entities.Characters;

namespace RPGCharacterService.Persistence.DynamoDb.Models {
  /// <summary>
  /// DynamoDB representation of a character.
  /// </summary>
  [DynamoDBTable("characters")]
  public class CharacterDocument {
    [DynamoDBHashKey]
    public required string Id { get; init; }

    [DynamoDBProperty]
    public required string Name { get; init; }

    [DynamoDBProperty]
    public required string Race { get; init; }

    [DynamoDBProperty]
    public string Subrace { get; init; } = string.Empty;

    [DynamoDBProperty]
    public required string Class { get; init; }

    [DynamoDBProperty]
    public int Level { get; init; } = 1;

    [DynamoDBProperty]
    public required int HitPoints { get; init; }

    [DynamoDBProperty]
    public Dictionary<string, int> AbilityScores { get; init; } = new();

    [DynamoDBProperty]
    public EquipmentDocument Equipment { get; init; } = new();

    [DynamoDBProperty]
    public WealthDocument Wealth { get; init; } = new();

    [DynamoDBProperty]
    public CharacterInitializationFlags InitFlags { get; init; } = CharacterInitializationFlags.None;
  }
  /// <summary>
  /// DynamoDB representation of a character's equipment.
  /// </summary>
  public class EquipmentDocument {
    [DynamoDBProperty]
    public ItemDocument? MainHand { get; init; }

    [DynamoDBProperty]
    public ItemDocument? OffHand { get; init; }

    [DynamoDBProperty]
    public ItemDocument? Armor { get; init; }
  }

  /// <summary>
  /// DynamoDB representation of a character's wealth.
  /// </summary>
  public class WealthDocument {
    [DynamoDBProperty]
    public int Copper { get; init; }

    [DynamoDBProperty]
    public int Silver { get; init; }

    [DynamoDBProperty]
    public int Electrum { get; init; }

    [DynamoDBProperty]
    public int Gold { get; init; }

    [DynamoDBProperty]
    public int Platinum { get; init; }
  }
}
