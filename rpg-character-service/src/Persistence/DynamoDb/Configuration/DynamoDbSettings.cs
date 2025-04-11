namespace RPGCharacterService.Persistence.DynamoDb.Configuration {
  /// <summary>
  /// DynamoDB settings class that reads from environment variables.
  /// This is used for configuration of the DynamoDB client.
  /// </summary>
  public class DynamoDbSettings {
    public string RegionName => Environment.GetEnvironmentVariable("DYNAMODB_REGION") ?? "us-east-1";
    public string ServiceUrl => Environment.GetEnvironmentVariable("DYNAMODB_SERVICE_URL") ?? "http://localhost:8000";
    public string DbPrefix => Environment.GetEnvironmentVariable("DYNAMODB_DB_PREFIX") ?? "dev-";
  }
}
