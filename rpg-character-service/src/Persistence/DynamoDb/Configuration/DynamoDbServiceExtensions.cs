using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using RPGCharacterService.Persistence.DynamoDb.Mapping;
using RPGCharacterService.Persistence.DynamoDb.Repositories;

namespace RPGCharacterService.Persistence.DynamoDb.Configuration {
  /// <summary>
  /// Extension methods for registering DynamoDB services.
  /// </summary>
  public static class DynamoDbServiceExtensions {
    /// <summary>
    /// Adds DynamoDB services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection ConfigureDynamoDbPersistence(this IServiceCollection services) {
      // Register settings
      services.AddScoped<DynamoDbSettings>();

      // Register DynamoDB client
      services.AddScoped<IAmazonDynamoDB>(sp => {
        var settings = sp.GetRequiredService<DynamoDbSettings>();
        var config = new AmazonDynamoDBConfig {
          RegionEndpoint = RegionEndpoint.GetBySystemName(settings.RegionName),
          ServiceURL = settings.ServiceUrl
        };

        return new AmazonDynamoDBClient(config);
      });

      // Register DB Context with environment-specific table names
      services.AddScoped<IDynamoDBContext>(sp => {
        var settings = sp.GetRequiredService<DynamoDbSettings>();
        var db = sp.GetRequiredService<IAmazonDynamoDB>();

        var contextConfig = new DynamoDBContextConfig {
          TableNamePrefix = $"{settings.DbPrefix}"
        };

        return new DynamoDBContext(db, contextConfig);
      });

      // Register repositories
      services.AddScoped<ICharacterRepository, DynamoDbCharacterRepository>();
      services.AddScoped<IItemRepository, DynamoDbItemRepository>();

      // Register initialization service
      services.AddScoped<DynamoDbInitializer>();
      services.AddScoped<DynamoDbSeeder>();

      // Register AutoMapper profiles
      services.AddAutoMapper(typeof(CharacterDocumentMappingProfile));
      services.AddAutoMapper(typeof(ItemDocumentMappingProfile));

      return services;
    }
  }
}
