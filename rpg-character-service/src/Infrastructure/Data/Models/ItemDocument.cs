using Amazon.DynamoDBv2.DataModel;
using RPGCharacterService.Entities.Items;

namespace RPGCharacterService.Infrastructure.Data.Models {
  /// <summary>
  /// DynamoDB representation of an item.
  /// </summary>
  [DynamoDBTable("items")]
  public class ItemDocument {
    [DynamoDBHashKey]
    public string Id { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string Name { get; set; } = string.Empty;

    [DynamoDBProperty]
    public EquipmentStatsDocument? EquipmentStats { get; set; }
  }

  /// <summary>
  /// DynamoDB representation of equipment stats.
  /// </summary>
  public class EquipmentStatsDocument {
    [DynamoDBProperty]
    public EquipmentType EquipmentType { get; set; }

    [DynamoDBProperty]
    public ArmorStatsDocument? ArmorStats { get; set; }

    [DynamoDBProperty]
    public WeaponStatsDocument? WeaponStats { get; set; }
  }

  /// <summary>
  /// DynamoDB representation of armor stats.
  /// </summary>
  public class ArmorStatsDocument {
    [DynamoDBProperty]
    public int BaseArmorClass { get; set; }

    [DynamoDBProperty]
    public ArmorType ArmorType { get; set; }
  }

  /// <summary>
  /// DynamoDB representation of weapon stats.
  /// </summary>
  public class WeaponStatsDocument {
    [DynamoDBProperty]
    public WeaponProperty WeaponProperties { get; set; }

    [DynamoDBProperty]
    public WeaponRangeType RangeType { get; set; }
  }
}
