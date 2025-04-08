using RPGCharacterService.Models.Items;

namespace RPGCharacterService.Persistence.Items
{
    public interface IItemRepository
    {
        Task<Item?> GetByIdAsync(Guid id);
        Task<IEnumerable<Item>> GetAllAsync();
        Task<Item> CreateAsync(Item item);
        Task<Item> UpdateAsync(Item item);
        Task DeleteAsync(Guid id);
    }
}
