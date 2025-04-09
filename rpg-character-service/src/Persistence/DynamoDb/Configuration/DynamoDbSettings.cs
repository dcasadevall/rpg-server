namespace RPGCharacterService.Persistence.DynamoDb.Configuration {
  /// <summary>
  /// DynamoDB settings class that reads from environment variables.
  /// This is used for configuration of the DynamoDB client.
  /// </summary>
  public class DynamoDbSettings {
    public string RegionName => Environment.GetEnvironmentVariable("DYNAMODB_REGION") ?? "us-east-2";
    public string ServiceUrl => Environment.GetEnvironmentVariable("DYNAMODB_SERVICE_URL") ?? "http://localhost:8000";
    public string AccessKey => Environment.GetEnvironmentVariable("DYNAMODB_ACCESS_KEY") ?? "dumm";
    public string SecretKey => Environment.GetEnvironmentVariable("DYNAMODB_SECRET_KEY") ?? "dummy";
  }
}
