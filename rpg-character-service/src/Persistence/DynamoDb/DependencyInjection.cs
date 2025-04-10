using Amazon.DynamoDBv2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RPGCharacterService.Persistence.DynamoDb.Configuration;

namespace RPGCharacterService.Persistence.DynamoDb
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDynamoDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DynamoDbConfig>(configuration.GetSection("DynamoDb"));

            services.AddAWSService<IAmazonDynamoDB>();
            services.AddScoped<DynamoDbContextFactory>();

            return services;
        }
    }
}
