using RPGCharacterService.Models.Characters;
using Swashbuckle.AspNetCore.Annotations;

namespace RPGCharacterService.Dtos.Equipment.Responses {
  /// <summary>
  ///   Response object for equipment-related operations
  /// </summary>
  public class EquipmentResponse {
    /// <summary>
    ///   The character's calculated armor class after equipment changes
    /// </summary>
    [SwaggerSchema(Description = "The character's current armor class after equipment changes")]
    public int ArmorClass { get; set; }

    /// <summary>
    ///   The ability score used for damage calculations
    /// </summary>
    [SwaggerSchema(Description = "The ability score used for weapon damage calculations")]
    public AbilityScore WeaponDamageModifier { get; set; }

    /// <summary>
    ///   Details about the character's equipment
    /// </summary>
    [SwaggerSchema(Description = "Details about the character's equipped items")]
    public EquipmentDetails Equipment { get; set; } = new();
  }
  /// <summary>
  ///   Contains details about the character's equipped items
  /// </summary>
  public class EquipmentDetails {
    /// <summary>
    ///   The ID of the armor equipped in the armor slot, if any
    /// </summary>
    [SwaggerSchema(Description = "The ID of the armor equipped in the armor slot, null if nothing equipped")]
    public int? Armor { get; set; }

    /// <summary>
    ///   The ID of the weapon equipped in the main hand slot, if any
    /// </summary>
    [SwaggerSchema(Description = "The ID of the weapon equipped in the main hand slot, null if nothing equipped")]
    public int? MainHand { get; set; }

    /// <summary>
    ///   The ID of the weapon or shield equipped in the off-hand slot, if any
    /// </summary>
    [SwaggerSchema(Description =
                      "The ID of the weapon or shield equipped in the off-hand slot, null if nothing equipped")]
    public int? OffHand { get; set; }
  }
}
