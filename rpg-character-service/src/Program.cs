using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using RPGCharacterService.Controllers;
using RPGCharacterService.Controllers.Filters;
using RPGCharacterService.Persistence.Characters;
using RPGCharacterService.Persistence.Items;
using RPGCharacterService.Services;
using RPGCharacterService.Infrastructure.Data;
using RPGCharacterService.Infrastructure.Extensions;
using DotNetEnv;

// Load environment variables from .env file
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to DI container
builder.Services.AddControllers(options => {
  // Ensures model state is valid for all requests,
  // and returns 400 Bad Request if not.
  options.Filters.Add(typeof(ValidateModelFilterAttribute));
  // Ensures unhandled exceptions return 500 Internal Server Error
  options.Filters.Add(typeof(ExceptionFilterAttribute));
});

// Get database configuration from environment variables
var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "rpg_character_service";
var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

if (string.IsNullOrEmpty(dbPassword))
{
    throw new InvalidOperationException("Database password is not configured. Please set DB_PASSWORD in your .env file.");
}

var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";

// Add database services based on environment
if (builder.Environment.IsDevelopment())
{
    // Use in-memory repositories for development
    builder.Services.AddInMemoryRepositories();
}
else
{
    // Use database repositories for production
    builder.Services.AddDatabase(connectionString);
}

builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IStatsService, StatsService>();
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddSingleton<IDiceService, DiceService>();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
  // Use Swagger Comments for better self-documentation
  c.EnableAnnotations();

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

  // Add future supported versions here.
  // This will generate multiple Swagger endpoints for different API versions.
});

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

// Enable Swagger Middleware
app.UseSwagger();
app.UseStaticFiles();
app.UseSwaggerUI(c => {
  // Serve each swagger version. Multiple versions would require adding
  // additional c.SwaggerEndpoint calls for each version.
  c.SwaggerEndpoint("/swagger/v1/swagger.json", "RPG Character Service API");

  // Serve Swagger UI at http://localhost:5266/
  c.RoutePrefix = "";
});

// Configure routing
app.UseRouting();
app.MapControllers();

app.Run();
