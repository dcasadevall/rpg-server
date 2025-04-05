var builder = WebApplication.CreateBuilder(args);

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable static files (important to serve openapi.json)
app.UseStaticFiles();

// Enable Swagger Middleware
app.UseSwagger(); // This will generate /swagger/v1/swagger.json dynamically
app.UseSwaggerUI(c =>
{
    // Point Swagger UI to your own static openapi.json file!
    c.SwaggerEndpoint("/openapi.json", "RPG Character Manager API");
    c.RoutePrefix = ""; // Serve Swagger UI at http://localhost:5266/
});

// Uncomment once controllers are set up.
// app.MapControllers();

app.Run();
