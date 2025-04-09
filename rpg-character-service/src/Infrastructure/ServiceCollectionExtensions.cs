using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RPGCharacterService.Infrastructure.Data;
using RPGCharacterService.Infrastructure.Data.Mapping;
using RPGCharacterService.Infrastructure.Data.Repositories;
using RPGCharacterService.Persistence.Characters;
using RPGCharacterService.Persistence.Items;

namespace RPGCharacterService.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext(connectionString);
            services.AddRepositories();
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            return services;
        }

        private static IServiceCollection AddDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<RpgDbContext>(options =>
                options.UseNpgsql(connectionString));

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<ICharacterRepository, EntityFrameworkCharacterRepository>();
            services.AddScoped<IItemRepository, EntityFrameworkItemRepository>();

            return services;
        }
    }
}
