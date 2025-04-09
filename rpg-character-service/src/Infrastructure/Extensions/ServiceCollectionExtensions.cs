using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RPGCharacterService.Infrastructure.Data;
using RPGCharacterService.Infrastructure.Data.Repositories;
using RPGCharacterService.Persistence.Characters;
using RPGCharacterService.Persistence.Items;

namespace RPGCharacterService.Infrastructure.Extensions
{
    /// <summary>
    /// Extension methods for IServiceCollection.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds database services to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            // Get the connection string from configuration
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Add the DbContext
            services.AddDbContext<RpgDbContext>(options =>
                options.UseNpgsql(connectionString, npgsqlOptions =>
                    npgsqlOptions.MigrationsAssembly("RPGCharacterService.Infrastructure")));

            // Add the repositories
            services.AddScoped<ICharacterRepository, EntityFrameworkCharacterRepository>();
            services.AddScoped<IItemRepository, EntityFrameworkItemRepository>();

            return services;
        }

        /// <summary>
        /// Adds in-memory repositories for local development and testing.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddInMemoryRepositories(this IServiceCollection services)
        {
            services.AddSingleton<ICharacterRepository, InMemoryCharacterRepository>();
            services.AddSingleton<IItemRepository, InMemoryItemRepository>();

            return services;
        }
    }
}
