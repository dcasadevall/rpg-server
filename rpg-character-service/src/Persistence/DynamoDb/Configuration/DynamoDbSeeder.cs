using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using RPGCharacterService.Entities.Items;
using RPGCharacterService.Persistence.DynamoDb.Models;

namespace RPGCharacterService.Persistence.DynamoDb.Configuration {
  /// <summary>
  /// Class used to initially seed dynamo db from a configuration json file.
  /// </summary>
  /// <param name="context"></param>
  /// <param name="logger"></param>
  public class DynamoDbSeeder(IDynamoDBContext context, ILogger<DynamoDbSeeder> logger) {
    public async Task SeedItemsAsync() {
      try {
        logger.LogInformation("Starting to seed items from JSON file");

        // Read the JSON file
        var jsonPath = Path.Combine(AppContext.BaseDirectory, "Persistence", "DynamoDb", "Configuration", "seed-data.json");
        logger.LogInformation("Looking for seed data file at: {Path}", jsonPath);

        if (!File.Exists(jsonPath)) {
          logger.LogError("Seed data file not found at: {Path}", jsonPath);
          return;
        }

        var jsonContent = await File.ReadAllTextAsync(jsonPath);
        logger.LogInformation("Read {Length} bytes from seed data file", jsonContent.Length);

        var options = new JsonSerializerOptions {
          PropertyNameCaseInsensitive = true
        };

        var seedData = JsonSerializer.Deserialize<SeedData>(jsonContent, options);
        logger.LogInformation("Deserialized seed data. Items count: {Count}", seedData?.Items?.Count ?? 0);

        if (seedData?.Items == null || !seedData.Items.Any()) {
          logger.LogWarning("No items found in seed data file");
          return;
        }

        // Check if items already exist
        var existingItems = await context
                                  .ScanAsync<ItemDocument>([])
                                  .GetRemainingAsync();
        if (existingItems.Any()) {
          logger.LogInformation("Items already exist in the database, skipping seeding");
          return;
        }

        // Convert and save items
        foreach (var item in seedData.Items) {
          var dynamoDbItem = new ItemDocument {
            Id = item.Id,
            Name = item.Name,
            EquipmentStats = new EquipmentStatsDocument {
              EquipmentType = Enum.Parse<EquipmentType>(item.EquipmentType),
              WeaponStats = item.WeaponStats != null ? new WeaponStatsDocument {
                WeaponProperties = item.WeaponStats.WeaponProperties
                                       .Select(Enum.Parse<WeaponProperty>)
                                       .Aggregate(WeaponProperty.None, (current, next) => current | next),
                RangeType = Enum.Parse<WeaponRangeType>(item.WeaponStats.RangeType)
              } : null,
              ArmorStats = item.ArmorStats != null ? new ArmorStatsDocument {
                BaseArmorClass = item.ArmorStats.BaseArmorClass,
                ArmorType = Enum.Parse<ArmorType>(item.ArmorStats.ArmorType)
              } : null
            }
          };

          await context.SaveAsync(dynamoDbItem);
          logger.LogInformation("Seeded item: {ItemName}", item.Name);
        }

        logger.LogInformation("Successfully seeded {Count} items", seedData.Items.Count);
      } catch (Exception ex) {
        logger.LogError(ex, "Error seeding items from JSON file");
        throw;
      }
    }

    private class SeedData {
      [JsonPropertyName("items")]
      public List<SeedItem> Items { get; set; } = new();
    }

    private class SeedItem {
      [JsonPropertyName("id")]
      public string Id { get; set; } = string.Empty;

      [JsonPropertyName("name")]
      public string Name { get; set; } = string.Empty;

      [JsonPropertyName("equipmentType")]
      public string EquipmentType { get; set; } = string.Empty;

      [JsonPropertyName("weaponStats")]
      public SeedWeaponStats? WeaponStats { get; set; }

      [JsonPropertyName("armorStats")]
      public SeedArmorStats? ArmorStats { get; set; }
    }

    private class SeedWeaponStats {
      [JsonPropertyName("weaponProperties")]
      public List<string> WeaponProperties { get; set; } = new();

      [JsonPropertyName("rangeType")]
      public string RangeType { get; set; } = string.Empty;
    }

    private class SeedArmorStats {
      [JsonPropertyName("baseArmorClass")]
      public int BaseArmorClass { get; set; }

      [JsonPropertyName("armorType")]
      public string ArmorType { get; set; } = string.Empty;
    }
  }
}
