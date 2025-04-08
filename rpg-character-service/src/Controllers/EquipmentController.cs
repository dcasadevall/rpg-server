using Microsoft.AspNetCore.Mvc;
using RPGCharacterService.Dtos.Equipment.Responses;
using RPGCharacterService.Exceptions.Character;
using RPGCharacterService.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace RPGCharacterService.Controllers {
  [ApiController]
  [ApiVersion("1.0")]
  [Route("api/v{version:apiVersion}/characters/{characterId:guid}/equipment")]
  public class EquipmentController(IEquipmentService equipmentService) : ControllerBase {
    [HttpPatch("armor/{armorId:int}")]
    [SwaggerOperation(Summary = "Equip Armor to a Character",
                       Description = "Equips the specified armor to the given character")]
    [SwaggerResponse(200, "Armor Equipped", typeof(EquipmentResponse))]
    [SwaggerResponse(400, "Invalid ID or Armor ID")]
    [SwaggerResponse(404, "Character or Armor Not Found")]
    public async Task<ActionResult<EquipmentResponse>> EquipArmor(
      [SwaggerParameter("Character identifier", Required = true)] Guid characterId,
      [SwaggerParameter("Armor item identifier", Required = true)] uint armorId) {
      try {
        var character = await equipmentService.EquipItemAsync(characterId, armorId, EquipmentSlot.Armor);
        return Ok(new EquipmentResponse {
          ArmorClass = character.ArmorClass,
          Equipment = new EquipmentDetails {
            Armor = character.EquippedItems.ArmorId,
            MainHand = character.EquippedItems.MainHandId,
            OffHand = character.EquippedItems.OffHandId
          }
        });
      } catch (CharacterNotFoundException) {
        return NotFound(new {error = "CHARACTER_NOT_FOUND", message = "Character not found."});
      } catch (InvalidEquipmentOperationException) {
        return NotFound(new {error = "ARMOR_NOT_FOUND", message = "Armor not found or cannot be equipped."});
      }
    }

    [HttpPatch("weapons/{weaponId:int}")]
    [SwaggerOperation(Summary = "Equip Weapon to a Character",
                       Description = "Equips the specified weapon to the given character")]
    [SwaggerResponse(200, "Weapon Equipped", typeof(EquipmentResponse))]
    [SwaggerResponse(400, "Invalid ID or Weapon ID")]
    [SwaggerResponse(404, "Character or Weapon Not Found")]
    public async Task<ActionResult<EquipmentResponse>> EquipWeapon(
      [SwaggerParameter("Character identifier", Required = true)] Guid characterId,
      [SwaggerParameter("Weapon item identifier", Required = true)] uint weaponId,
      [FromBody] [SwaggerRequestBody("Off-hand weapon details", Required = false)] EquipWeaponRequest request) {
      try {
        var isOffHand = request?.OffHand ?? false;
        var slot = isOffHand ? EquipmentSlot.OffHand : EquipmentSlot.MainHand;

        var character = await equipmentService.EquipItemAsync(characterId, weaponId, slot);
        return Ok(new EquipmentResponse {
          ArmorClass = character.ArmorClass,
          Equipment = new EquipmentDetails {
            MainHand = character.EquippedItems.MainHandId,
            OffHand = character.EquippedItems.OffHandId,
            Armor = character.EquippedItems.ArmorId
          }
        });
      } catch (CharacterNotFoundException) {
        return NotFound(new {error = "CHARACTER_NOT_FOUND", message = "Character not found."});
      } catch (InvalidEquipmentOperationException) {
        return NotFound(new {error = "WEAPON_NOT_FOUND", message = "Weapon not found or cannot be equipped."});
      }
    }

    public class EquipWeaponRequest {
      [SwaggerSchema(Description = "Whether to equip the weapon in the off-hand slot")]
      public bool OffHand { get; set; }
    }
  }
}
