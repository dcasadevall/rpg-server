namespace RPGCharacterService.Infrastructure.Data.Entities {
  /// <summary>
  /// Database entity representing a character's equipment.
  /// </summary>
  public class CharacterEquipmentEntity {
    /// <summary>
    /// The unique identifier for this equipment record.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The ID of the character this equipment belongs to.
    /// </summary>
    public Guid CharacterId { get; set; }

    /// <summary>
    /// The ID of the item equipped in the main hand, if any.
    /// </summary>
    public int? MainHandItemId { get; set; }

    /// <summary>
    /// The ID of the item equipped in the off hand, if any.
    /// </summary>
    public int? OffHandItemId { get; set; }

    /// <summary>
    /// The ID of the armor item, if any.
    /// </summary>
    public int? ArmorItemId { get; set; }

    // Navigation properties

    /// <summary>
    /// The character this equipment belongs to.
    /// </summary>
    public CharacterEntity Character { get; set; } = null!;

    /// <summary>
    /// The item equipped in the main hand, if any.
    /// </summary>
    public ItemEntity? MainHandItem { get; set; }

    /// <summary>
    /// The item equipped in the off hand, if any.
    /// </summary>
    public ItemEntity? OffHandItem { get; set; }

    /// <summary>
    /// The armor item, if any.
    /// </summary>
    public ItemEntity? ArmorItem { get; set; }
  }
}
