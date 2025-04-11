using System.Reflection;
using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using RPGCharacterService.Controllers.Filters;
using RPGCharacterService.Services;
using RPGCharacterService.Dtos.Mapping;
using RPGCharacterService.Mapping;
using RPGCharacterService.Persistence.DynamoDb.Configuration;
using RPGCharacterService.Persistence.InMemory;

namespace RPGCharacterService;

public class Program {
  public static async Task Main(string[] args) {
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllers(options => {
      // Ensures model state is valid for all requests,
      // and returns 400 Bad Request if not.
      options.Filters.Add(typeof(ValidateModelFilterAttribute));
      // Ensures unhandled exceptions return 500 Internal Server Error
      options.Filters.Add(typeof(ExceptionFilterAttribute));
    });

    // Configure Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c => {
      // Use Swagger Comments for better self-documentation
      c.EnableAnnotations();

      // More API Versions can be added in the future, with additional SwaggerDoc
      c.SwaggerDoc("v1",
                   new OpenApiInfo {
                     Version = "v1",
                     Title = "RPG Character Service API",
                     Description = "A RESTful service for managing D&D-inspired RPG characters."
                   });


      // Include XML comments
      var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
      var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
      c.IncludeXmlComments(xmlPath);
    });

    // Configure in-memory database for local development
    // Otherwise use dynamo db
    Env.Load();
    var inMemoryDb = Env.GetString("DB_TYPE", "in-memory") == "in-memory";
    if (inMemoryDb) {
      Console.WriteLine("Configuring in-memory database");
      builder.Services.ConfigureInMemoryPersistence();
    } else {
      Console.WriteLine("Configuring DynamoDB database");
      builder.Services.ConfigureDynamoDbPersistence();
    }

    // Add Domain dependencies
    builder.Services.AddScoped<ICharacterService, CharacterService>();
    builder.Services.AddScoped<ICurrencyService, CurrencyService>();
    builder.Services.AddScoped<IStatsService, StatsService>();
    builder.Services.AddScoped<IEquipmentService, EquipmentService>();
    builder.Services.AddScoped<IDiceService, DiceService>();
    builder.Services.AddScoped<ICharacterValidator, HardcodedCharacterValidator>();
    builder.Services.AddScoped<LoggedMapper>();

    // Add Dto Automapper
    builder.Services.AddAutoMapper(typeof(DtoMappingProfile).Assembly);

    // Setup Api Versioning
    builder.Services.AddApiVersioning(options => {
      // Default to v1 and assume default version when unspecified
      options.DefaultApiVersion = new ApiVersion(1, 0);
      options.AssumeDefaultVersionWhenUnspecified = true;

      // Adds API versions to response headers
      options.ReportApiVersions = true;
    });

    // Setup Versioned API Explorer.
    // Allows swagger to pick up and display different versions in the UI
    // This would also allow us to generate different docs for different API versions
    builder.Services.AddVersionedApiExplorer(options => {
      options.GroupNameFormat = "'v'VVV"; // This formats versions like v1, v2
      options.SubstituteApiVersionInUrl = true; // Substitute {version} if used
    });

    var app = builder.Build();

    // Initialize DynamoDB tables and seed data in non-development environments
    if (!inMemoryDb) {
      using var scope = app.Services.CreateScope();
      var dynamoDbInitializer = scope.ServiceProvider.GetRequiredService<DynamoDbInitializer>();
      await dynamoDbInitializer.InitializeTablesAsync();
    }

    // Configure the HTTP request pipeline.
    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "dev";
    if (environment is "dev" or "local") {
      app.UseSwagger();
      app.UseSwaggerUI(c => {
        // Serve each swagger version. Multiple versions would require adding
        // additional c.SwaggerEndpoint calls for each version.
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RPG Character Service API");

        // Serve Swagger UI at http://localhost:5266/
        c.RoutePrefix = "";
      });
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    await app.RunAsync();
  }
}
