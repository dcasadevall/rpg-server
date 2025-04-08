using Swashbuckle.AspNetCore.Annotations;

namespace RPGCharacterService.Dtos.Equipment {
  /// <summary>
  ///   Request object for equipping a weapon to a character
  /// </summary>
  public class EquipWeaponRequest {
    /// <summary>
    ///   Whether to equip the weapon in the off-hand slot
    /// </summary>
    [SwaggerSchema(Description = "When true, equips the weapon to the off-hand slot. When false, equips to main hand.")]
    public bool? OffHand { get; set; } = false;
  }
}
