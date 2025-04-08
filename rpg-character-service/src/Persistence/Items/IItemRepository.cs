using RPGCharacterService.Exceptions.Items;
using RPGCharacterService.Models.Items;

namespace RPGCharacterService.Persistence.Items {
  public interface IItemRepository {
    Task<Item?> GetByIdAsync(int id);
    Task<Item> CreateAsync(Item item);
    Task<Item> UpdateAsync(Item item);
    Task DeleteAsync(Guid id);
  }

  public static class ItemRepositoryExtensions {
    public static async Task<Item> GetByIdOrThrowAsync(this IItemRepository repository, int id) {
      var item = await repository.GetByIdAsync(id);
      if (item == null) {
        throw new ItemNotFoundException(id);
      }

      return item;
    }
  }
}
