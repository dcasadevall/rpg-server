using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace RPGCharacterService.Infrastructure.Data;

/// <summary>
/// Service for initializing DynamoDB tables.
/// </summary>
public class DynamoDbInitializationService(IAmazonDynamoDB client, DynamoDbSettings settings) {
  /// <summary>
  /// Initializes the DynamoDB tables.
  /// </summary>
  public async Task InitializeTablesAsync() {
    var tables = await client.ListTablesAsync();

    // Initialize characters table
    await InitializeTableAsync($"{settings.TablePrefix}characters");

    // Initialize items table
    await InitializeTableAsync($"{settings.TablePrefix}items");
  }

  private async Task InitializeTableAsync(string tableName) {
    var tables = await client.ListTablesAsync();

    if (!tables.TableNames.Contains(tableName)) {
      var request = new CreateTableRequest {
        TableName = tableName,
        AttributeDefinitions = new List<AttributeDefinition> {
          new AttributeDefinition {
            AttributeName = "Id",
            AttributeType = "S"
          }
        },
        KeySchema = new List<KeySchemaElement> {
          new KeySchemaElement {
            AttributeName = "Id",
            KeyType = "HASH"
          }
        },
        ProvisionedThroughput = new ProvisionedThroughput {
          ReadCapacityUnits = 5,
          WriteCapacityUnits = 5
        }
      };

      await client.CreateTableAsync(request);

      // Wait for the table to be created
      var describeRequest = new DescribeTableRequest {TableName = tableName};
      bool tableActive = false;

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
