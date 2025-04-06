using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using RPGCharacterService.Persistence;
using RPGCharacterService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to DI container
builder.Services.AddControllers();
builder.Services.AddSingleton<ICharacterRepository, InMemoryCharacterRepository>();
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IStatsService, StatsService>();
builder.Services.AddSingleton<IDiceService, DiceService>();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Use Swagger Comments for better self-documentation
    c.EnableAnnotations();
    
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "RPG Character Service API",
        Description = "A RESTful service for managing D&D-inspired RPG characters."
    });

    // Add future supported versions here.
    // This will generate multiple Swagger endpoints for different API versions.
});

// Setup Api Versioning
builder.Services.AddApiVersioning(options =>
{
    // Default to v1 and assume default version when unspecified
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    
    // Adds API versions to response headers
    options.ReportApiVersions = true; 
});

// Setup Versioned API Explorer.
// Allows swagger to pick up and display different versions in the UI
// This would also allow us to generate different docs for different API versions
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // This formats versions like v1, v2
    options.SubstituteApiVersionInUrl = true; // Substitute {version} if used
});

var app = builder.Build();

// Enable Swagger Middleware
app.UseSwagger();
app.UseStaticFiles();
app.UseSwaggerUI(c =>
{
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
