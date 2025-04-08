using RPGCharacterService.Models.Items;

namespace RPGCharacterService.Persistence.Items {
    /// <summary>
    /// In-memory implementation of IItemRepository for local testing and development.
    /// This implementation stores items in a dictionary and is not suitable for production use
    /// as it does not persist data between application restarts and is not thread-safe.
    /// </summary>
    public class InMemoryItemRepository : IItemRepository {
        private readonly Dictionary<int, Item> items = new();

        /// <summary>
        /// Retrieves an item by its unique identifier from the in-memory store.
        /// </summary>
        /// <param name="id">The unique identifier of the item.</param>
        /// <returns>The item if found, null otherwise.</returns>
        public Task<Item?> GetByIdAsync(int id) {
            return Task.FromResult(items.GetValueOrDefault(id));
        }
    }
}
