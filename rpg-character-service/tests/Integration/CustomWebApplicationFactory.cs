namespace RPGCharacterService.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program> {
  protected override void ConfigureWebHost(IWebHostBuilder builder) {
    builder.UseEnvironment("Local");

    // Replace services for testing
    builder.ConfigureServices(services => {
      // You can add/replace services here for testing
    });
  }
}
