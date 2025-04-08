using RPGCharacterService.Exceptions.Items;
using RPGCharacterService.Models.Items;

namespace RPGCharacterService.Persistence.Items {
  /// <summary>
  /// Interface defining the contract for item persistence operations.
  /// </summary>
  public interface IItemRepository {
    /// <summary>
    /// Retrieves an item by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the item.</param>
    /// <returns>The item if found, null otherwise.</returns>
    Task<Item?> GetByIdAsync(int id);
  }

  public static class ItemRepositoryExtensions {
    /// <summary>
    /// Retrieves an item by its unique identifier, throwing an exception if it is not found.
    /// </summary>
    /// <param name="repository">The item repository.</param>
    /// <param name="id">The unique identifier of the item.</param>
    /// <returns>The item if found, null otherwise.</returns>
    public static async Task<Item> GetByIdOrThrowAsync(this IItemRepository repository, int id) {
      var item = await repository.GetByIdAsync(id);
      if (item == null) {
        throw new ItemNotFoundException(id);
      }

      return item;
    }
  }
}
