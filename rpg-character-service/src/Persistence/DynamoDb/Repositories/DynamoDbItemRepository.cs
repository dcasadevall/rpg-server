using Amazon.DynamoDBv2.DataModel;
using AutoMapper;
using RPGCharacterService.Entities.Items;
using RPGCharacterService.Persistence.DynamoDb.Models;

namespace RPGCharacterService.Persistence.DynamoDb.Repositories {
  /// <summary>
  /// DynamoDB implementation of the item repository.
  /// </summary>
  public class DynamoDbItemRepository(IDynamoDBContext context, IMapper mapper) : IItemRepository {
    /// <inheritdoc/>
    public async Task<Item?> GetByIdAsync(int id) {
      var item = await context.LoadAsync<ItemDocument>(id.ToString());
      return item != null ? mapper.Map<Item>(item) : null;
    }
  }
}
