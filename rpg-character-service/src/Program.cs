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
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable static files (important to serve openapi.json)
app.UseStaticFiles();

// Enable Swagger Middleware
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    // Point Swagger UI to the local OpenAPI JSON
    c.SwaggerEndpoint("/openapi.json", "RPG Character Service API");
    // Serve Swagger UI at http://localhost:5266/
    c.RoutePrefix = ""; 
});

// Configure routing
app.UseRouting();
app.MapControllers();

app.Run();
