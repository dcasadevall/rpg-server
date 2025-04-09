using Amazon.DynamoDBv2.DataModel;
using RPGCharacterService.Entities.Items;

namespace RPGCharacterService.Persistence.DynamoDb.Models {
  /// <summary>
  /// DynamoDB representation of an item.
  /// </summary>
  [DynamoDBTable("items")]
  public class ItemDocument {
    [DynamoDBHashKey]
    public required string Id { get; init; }

    [DynamoDBProperty]
    public required string Name { get; init; }

    [DynamoDBProperty]
    public EquipmentStatsDocument? EquipmentStats { get; init; }
  }

  /// <summary>
  /// DynamoDB representation of equipment stats.
  /// </summary>
  public class EquipmentStatsDocument {
    [DynamoDBProperty]
    public EquipmentType EquipmentType { get; init; }

    [DynamoDBProperty]
    public ArmorStatsDocument? ArmorStats { get; init; }

    [DynamoDBProperty]
    public WeaponStatsDocument? WeaponStats { get; init; }
  }

  /// <summary>
  /// DynamoDB representation of armor stats.
  /// </summary>
  public class ArmorStatsDocument {
    [DynamoDBProperty]
    public int BaseArmorClass { get; init; }

    [DynamoDBProperty]
    public ArmorType ArmorType { get; init; }
  }

  /// <summary>
  /// DynamoDB representation of weapon stats.
  /// </summary>
  public class WeaponStatsDocument {
    [DynamoDBProperty]
    public WeaponProperty WeaponProperties { get; init; } = WeaponProperty.None;

    [DynamoDBProperty]
    public WeaponRangeType RangeType { get; init; } = WeaponRangeType.Melee;
  }
}
