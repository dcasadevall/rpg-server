using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using RPGCharacterService.Persistence.DynamoDb.Models;

namespace RPGCharacterService.Persistence.DynamoDb.Configuration {
  /// <summary>
  /// Service for initializing DynamoDB tables.
  /// This is useful for local setups where terraform or other infrastructure tools are not used.
  /// It is harmless to run this in production, but it is not necessary.
  /// Note that table names are hardcoded (here and in the document files), but in a real project
  /// we would use environment variables or configuration files to set the table names.
  /// </summary>
  public class DynamoDbInitializer(IAmazonDynamoDB client,
                                   DynamoDbSeeder seeder,
                                   ILogger<DynamoDbInitializer> logger,
                                   DynamoDbSettings settings) {
    /// <summary>
    /// Initializes the DynamoDB tables.
    /// </summary>
    public async Task InitializeTablesAsync() {
      // Initialize characters table
      logger.LogInformation("Initializing characters table");
      await InitializeTableAsync($"{settings.DbPrefix}characters");

      // Initialize items table
      logger.LogInformation("Initializing items table");
      await InitializeTableAsync($"{settings.DbPrefix}items");

      // Seed data if necessary
      logger.LogInformation("Seeding items");
      await seeder.SeedItemsAsync();
    }

    private async Task InitializeTableAsync(string tableName) {
      var tables = await client.ListTablesAsync();

      if (!tables.TableNames.Contains(tableName)) {
        var request = new CreateTableRequest {
          TableName = tableName,
          AttributeDefinitions = [
            new AttributeDefinition() {
              AttributeName = "Id",
              AttributeType = "S"
            }
          ],
          KeySchema = [
            new KeySchemaElement() {
              AttributeName = "Id",
              KeyType = "HASH"
            }
          ],
          ProvisionedThroughput = new ProvisionedThroughput {
            ReadCapacityUnits = 5,
            WriteCapacityUnits = 5
          }
        };

        await client.CreateTableAsync(request);

        // Wait for the table to be created
        var describeRequest = new DescribeTableRequest {TableName = tableName};
        var tableActive = false;

        while (!tableActive) {
          var tableDescription = await client.DescribeTableAsync(describeRequest);
          tableActive = tableDescription.Table.TableStatus == "ACTIVE";

          if (!tableActive) {
            await Task.Delay(1000);
          }
        }
      }
    }
  }
}
