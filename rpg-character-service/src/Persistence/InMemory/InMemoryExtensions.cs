namespace RPGCharacterService.Persistence.InMemory {
  public static class InMemoryExtensions {
    /// <summary>
    /// Adds the in-memory repositories to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection ConfigureInMemoryPersistence(this IServiceCollection services) {
      services.AddSingleton<ICharacterRepository, InMemoryCharacterRepository>();
      services.AddSingleton<IItemRepository, InMemoryItemRepository>();

      return services;
    }
  }
}
