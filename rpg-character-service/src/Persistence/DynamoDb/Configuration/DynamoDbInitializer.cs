using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace RPGCharacterService.Persistence.DynamoDb.Configuration {
  /// <summary>
  /// Service for initializing DynamoDB tables.
  /// This is useful for local setups where terraform or other infrastructure tools are not used.
  /// It is harmless to run this in production, but it is not necessary.
  /// Note that table names are hardcoded (here and in the document files), but in a real project
  /// we would use environment variables or configuration files to set the table names.
  /// </summary>
  public class DynamoDbInitializer(
    IAmazonDynamoDB client,
    ILogger<DynamoDbInitializer> logger,
    DynamoDbSeeder seeder,
    DynamoDbSettings settings) {
    /// <summary>
    /// Initializes the DynamoDB tables.
    /// </summary>
    public async Task InitializeTablesAsync() {
      var charTableRequest = new CreateTableRequest {
        TableName = $"{settings.DbPrefix}characters",
        AttributeDefinitions = [
          new AttributeDefinition {
            AttributeName = "Id",
            AttributeType = "S"
          },
        ],
        KeySchema = [
          new KeySchemaElement {
            AttributeName = "Id",
            KeyType = "HASH"
          },
        ],
        ProvisionedThroughput = new ProvisionedThroughput {
          ReadCapacityUnits = 5,
          WriteCapacityUnits = 5
        }
      };

      var itemTableRequest = new CreateTableRequest {
        TableName = $"{settings.DbPrefix}items",
        AttributeDefinitions = [
          new AttributeDefinition {
            AttributeName = "Id",
            AttributeType = "N"
          },
        ],
        KeySchema = [
          new KeySchemaElement {
            AttributeName = "Id",
            KeyType = "HASH"
          },
        ],
        ProvisionedThroughput = new ProvisionedThroughput {
          ReadCapacityUnits = 5,
          WriteCapacityUnits = 5
        }
      };

      await InitializeTablesAsync([charTableRequest, itemTableRequest]);

      // Seed data if necessary
      logger.LogInformation("Seeding items");
      await seeder.SeedItemsAsync();
    }

    private async Task InitializeTablesAsync(IEnumerable<CreateTableRequest> requests) {
      var tables = await client.ListTablesAsync();
      foreach (var request in requests) {
        logger.LogInformation($"Initializing {request.TableName}");
        if (tables.TableNames.Contains(request.TableName)) {
          continue;
        }

        logger.LogInformation($"Creating {request.TableName}");
        await client.CreateTableAsync(request);

        // Wait for the table to be created
        var describeRequest = new DescribeTableRequest {TableName = request.TableName};
        var tableActive = false;

        while (!tableActive) {
          var tableDescription = await client.DescribeTableAsync(describeRequest);
          tableActive = tableDescription.Table.TableStatus == "ACTIVE";

          if (!tableActive) {
            await Task.Delay(1000);
          }
        }

        logger.LogInformation($"Table {request.TableName} created and active.");
      }
    }
  }
}
