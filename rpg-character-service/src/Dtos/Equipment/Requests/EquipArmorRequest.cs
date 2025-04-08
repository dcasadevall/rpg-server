using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace RPGCharacterService.Dtos.Equipment.Requests {
  /// <summary>
  ///   Request object for equipping armor to a character
  /// </summary>
  public class EquipArmorRequest {
    /// <summary>
    ///   The unique identifier of the armor to equip
    /// </summary>
    [Required]
    [SwaggerSchema(Description = "The unique identifier of the armor item to equip")]
    public uint ArmorId { get; set; }
  }
}
