using Amazon;
using Amazon.DynamoDBv2;
using RPGCharacterService.Infrastructure.Data;
using RPGCharacterService.Infrastructure.Data.Mapping;
using RPGCharacterService.Persistence.Characters;
using RPGCharacterService.Persistence.Items;

namespace RPGCharacterService.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering DynamoDB services.
/// </summary>
public static class DynamoDbServiceExtensions {
  /// <summary>
  /// Adds DynamoDB services to the service collection.
  /// </summary>
  /// <param name="services">The service collection.</param>
  /// <returns>The service collection.</returns>
  public static IServiceCollection AddDynamoDb(this IServiceCollection services) {
    // Register settings
    services.AddScoped<DynamoDbSettings>();

    // Register DynamoDB client
    services.AddScoped<IAmazonDynamoDB>(sp => {
      var settings = sp.GetRequiredService<DynamoDbSettings>();
      var config = new AmazonDynamoDBConfig {
        RegionEndpoint = RegionEndpoint.GetBySystemName(settings.RegionName),
        ServiceURL = settings.ServiceUrl
      };

      return new AmazonDynamoDBClient(settings.AccessKey, settings.SecretKey, config);
    });

    // Register context and repositories
    services.AddScoped<DynamoDbContext>();
    services.AddScoped<ICharacterRepository, DynamoDbCharacterRepository>();
    services.AddScoped<IItemRepository, DynamoDbItemRepository>();

    // Register initialization service
    services.AddSingleton<DynamoDbInitializationService>();

    // Register AutoMapper profiles
    services.AddAutoMapper(typeof(CharacterDocumentMappingProfile));
    services.AddAutoMapper(typeof(ItemDocumentMappingProfile));

    return services;
  }
}
