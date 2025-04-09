using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using RPGCharacterService.Entities.Characters;
using RPGCharacterService.Mapping;
using RPGCharacterService.Persistence.DynamoDb.Models;

namespace RPGCharacterService.Persistence.DynamoDb.Repositories {
  /// <summary>
  /// DynamoDB implementation of the character repository.
  /// </summary>
  public class DynamoDbCharacterRepository(IDynamoDBContext context, LoggedMapper mapper) : ICharacterRepository {
    /// <inheritdoc/>
    public async Task<IEnumerable<Character>> GetAllAsync() {
      var documents = await context
                            .ScanAsync<CharacterDocument>([])
                            .GetRemainingAsync();
      return mapper.Map<IEnumerable<Character>>(documents);
    }

    /// <inheritdoc/>
    public async Task<Character?> GetByIdAsync(Guid id) {
      var item = await context.LoadAsync<CharacterDocument>(id.ToString());
      return item != null ? mapper.Map<Character>(item) : null;
    }

    /// <inheritdoc/>
    public async Task<Character?> GetByNameAsync(string name) {
      var scanConditions = new List<ScanCondition> {
        new("Name", ScanOperator.Equal, name)
      };

      var items = await context
                        .ScanAsync<CharacterDocument>(scanConditions)
                        .GetRemainingAsync();
      var item = items.FirstOrDefault();

      return item != null ? mapper.Map<Character>(item) : null;
    }

    /// <inheritdoc/>
    public async Task AddAsync(Character character) {
      var item = mapper.Map<CharacterDocument>(character);
      await context.SaveAsync(item);
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(Character character) {
      var item = mapper.Map<CharacterDocument>(character);
      await context.SaveAsync(item);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(Guid id) {
      await context.DeleteAsync<CharacterDocument>(id.ToString());
    }
  }
}
