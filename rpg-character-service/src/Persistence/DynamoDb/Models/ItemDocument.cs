using Amazon.DynamoDBv2.DataModel;
using RPGCharacterService.Entities.Items;
using RPGCharacterService.Persistence.DynamoDb.Configuration;

namespace RPGCharacterService.Persistence.DynamoDb.Models {
  /// <summary>
  /// DynamoDB representation of an item.
  /// Note that table names are hardcoded, but in a real project
  /// we would use environment variables or configuration files to set the table names.
  /// </summary>
  [DynamoDBTable("items")]
  public class ItemDocument {
    [DynamoDBHashKey]
    public required int Id { get; init; }

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
