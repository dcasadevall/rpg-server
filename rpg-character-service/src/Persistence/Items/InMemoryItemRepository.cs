using RPGCharacterService.Models.Items;

namespace RPGCharacterService.Persistence.Items {
    public class InMemoryItemRepository : IItemRepository {
        private readonly Dictionary<int, Item> _items = new();
        private int _nextId = 1;

        public Task<Item?> GetByIdAsync(int id) {
            return Task.FromResult(_items.GetValueOrDefault(id));
        }

        public Task<Item> CreateAsync(Item item) {
            var newItem = item with { Id = _nextId++ };
            _items[newItem.Id] = newItem;
            return Task.FromResult(newItem);
        }

        public Task<Item> UpdateAsync(Item item) {
            if (!_items.ContainsKey(item.Id)) {
                throw new KeyNotFoundException($"Item with ID {item.Id} not found");
            }

            _items[item.Id] = item;
            return Task.FromResult(item);
        }
    }
}
