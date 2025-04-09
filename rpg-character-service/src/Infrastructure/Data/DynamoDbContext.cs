using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace RPGCharacterService.Infrastructure.Data;

/// <summary>
/// DynamoDB context for managing database connections.
/// </summary>
public class DynamoDbContext
{
    private readonly DynamoDBContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamoDbContext"/> class.
    /// </summary>
    /// <param name="settings">The DynamoDB settings.</param>
    public DynamoDbContext(DynamoDbSettings settings)
    {
        var config = new AmazonDynamoDBConfig
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(settings.RegionName),
            ServiceURL = settings.ServiceUrl
        };

        var client = new AmazonDynamoDBClient(settings.AccessKey, settings.SecretKey, config);
        _context = new DynamoDBContext(client);
    }

    /// <summary>
    /// Gets the DynamoDB context.
    /// </summary>
    public DynamoDBContext Context => _context;
}

/// <summary>
/// DynamoDB settings class that reads from environment variables.
/// </summary>
public class DynamoDbSettings
{
    public string RegionName => Environment.GetEnvironmentVariable("DYNAMODB_REGION") ?? "us-east-1";
    public string ServiceUrl => Environment.GetEnvironmentVariable("DYNAMODB_SERVICE_URL") ?? "http://localhost:8000";
    public string AccessKey => Environment.GetEnvironmentVariable("DYNAMODB_ACCESS_KEY") ?? "dummy";
    public string SecretKey => Environment.GetEnvironmentVariable("DYNAMODB_SECRET_KEY") ?? "dummy";
    public string TablePrefix => Environment.GetEnvironmentVariable("DYNAMODB_TABLE_PREFIX") ?? "rpg-character-service-";
}
