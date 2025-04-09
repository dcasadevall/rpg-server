using Microsoft.EntityFrameworkCore;
using RPGCharacterService.Infrastructure.Data;
using RPGCharacterService.Infrastructure.Data.Mapping;
using RPGCharacterService.Persistence.Characters;
using RPGCharacterService.Persistence.Items;

namespace RPGCharacterService.Infrastructure.Extensions {
  public static class ServiceCollectionExtensions {
    /// <summary>
    /// Adds the infrastructure namespace related dependencies to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services,
                                                                   string connectionString) {
      // Set the repositories for the application to ECF repositories
      services.AddScoped<ICharacterRepository, EntityFrameworkCharacterRepository>();
      services.AddScoped<IItemRepository, EntityFrameworkItemRepository>();

      // Configure the DbContext to use the given connection string
      services.AddDbContext<RpgDbContext>(options => options.UseNpgsql(connectionString,
                                                                       npgsqlOptions =>
                                                                         npgsqlOptions
                                                                           .MigrationsAssembly("RPGCharacterService.Infrastructure")));

      // Add AutoMapper for mapping between entities and models
      services.AddAutoMapper(typeof(MappingProfile).Assembly);

      return services;
    }
  }
}
