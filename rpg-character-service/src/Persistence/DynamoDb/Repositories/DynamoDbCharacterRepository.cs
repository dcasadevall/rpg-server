using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using AutoMapper;
using RPGCharacterService.Entities.Characters;
using RPGCharacterService.Persistence.DynamoDb.Models;

namespace RPGCharacterService.Persistence.DynamoDb;

/// <summary>
/// DynamoDB implementation of the character repository.
/// </summary>
public class DynamoDbCharacterRepository(IDynamoDBContext context,
                                         IMapper mapper,
                                         ILogger<DynamoDbCharacterRepository> logger) : ICharacterRepository {
  /// <inheritdoc/>
  public async Task<IEnumerable<Character>> GetAllAsync() {
    var documents = await context
                          .ScanAsync<CharacterDocument>([])
                          .GetRemainingAsync();

    try {
      return mapper.Map<IEnumerable<Character>>(documents);
    } catch (AutoMapperMappingException ex) {
      logger.LogError(ex, "Error mapping DynamoDB documents to characters");
      throw new Exception("Error mapping DynamoDB documents to characters", ex);
    }
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
    var item = mapper.Map<CharacterDocument>(character.Id);
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
