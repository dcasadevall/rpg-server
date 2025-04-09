using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using RPGCharacterService.Entities.Items;
using RPGCharacterService.Persistence.DynamoDb.Models;

namespace RPGCharacterService.Persistence.DynamoDb.Configuration {
  public class DynamoDbSeeder(IAmazonDynamoDB dynamoDb, IDynamoDBContext context, ILogger<DynamoDbSeeder> logger) {
    public async Task SeedItemsAsync() {
      try {
        logger.LogInformation("Starting to seed items from JSON file");

        // Read the JSON file
        var jsonPath = Path.Combine(AppContext.BaseDirectory, "Configuration", "seed-data.json");
        var jsonContent = await File.ReadAllTextAsync(jsonPath);
        var seedData = JsonSerializer.Deserialize<SeedData>(jsonContent);

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
      public List<SeedItem> Items { get; set; } = new();
    }

    private class SeedItem {
      public string Id { get; set; } = string.Empty;
      public string Name { get; set; } = string.Empty;
      public string EquipmentType { get; set; } = string.Empty;
      public SeedWeaponStats? WeaponStats { get; set; }
      public SeedArmorStats? ArmorStats { get; set; }
    }

    private class SeedWeaponStats {
      public List<string> WeaponProperties { get; set; } = new();
      public string RangeType { get; set; } = string.Empty;
    }

    private class SeedArmorStats {
      public int BaseArmorClass { get; set; }
      public string ArmorType { get; set; } = string.Empty;
    }
  }
}
