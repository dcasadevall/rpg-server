using Microsoft.AspNetCore.Mvc;
using RPGCharacterService.Dtos.Equipment.Requests;
using RPGCharacterService.Dtos.Equipment.Responses;
using RPGCharacterService.Exceptions.Character;
using RPGCharacterService.Exceptions.Items;
using RPGCharacterService.Models.Characters;
using RPGCharacterService.Models.Items;
using RPGCharacterService.Rules;
using RPGCharacterService.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace RPGCharacterService.Controllers {
  [ApiController]
  [ApiVersion("1.0")]
  [Route("api/v{version:apiVersion}/characters/{characterId:guid}/equipment")]
  public class EquipmentController(IEquipmentService equipmentService,
                                   ICharacterRules characterRules,
                                   IEquipmentRules equipmentRules) : ControllerBase {
    [HttpPatch("armor/{armorId:int}")]
    [SwaggerOperation(Summary = "Equip Armor to a Character",
                       Description = "Equips the specified armor to the given character")]
    [SwaggerResponse(200, "Armor Equipped", typeof(EquipmentResponse))]
    [SwaggerResponse(400, "Invalid ID or Armor ID")]
    [SwaggerResponse(404, "Character or Armor Not Found")]
    public async Task<ActionResult<EquipmentResponse>> EquipArmor(
      [SwaggerParameter("Character identifier", Required = true)] Guid characterId,
      [SwaggerParameter("Armor item identifier", Required = true)] int armorId) {
      try {
        var character = await equipmentService.EquipArmorAsync(characterId, armorId);
        return Ok(new EquipmentResponse {
          ArmorClass = equipmentRules.CalculateArmorClass(character),
          Equipment = new EquipmentDetails {
            ArmorId = character.Equipment.Armor?.Id,
            MainHandId = character.Equipment.MainHand?.Id,
            OffHandId = character.Equipment.OffHand?.Id
          }
        });
      } catch (CharacterNotFoundException) {
        return NotFound(new {error = "CHARACTER_NOT_FOUND", message = "Character not found."});
      } catch (ItemNotFoundException) {
        return NotFound(new {error = "ITEM_NOT_FOUND", message = "The given item was not found."});
      } catch (EquipmentTypeMismatchException) {
        return NotFound(new {error = "EQUIPMENT_MISMATCH", message = "The given item is not armor."});
      }
    }

    [HttpPatch("weapon/{weaponId:int}")]
    [SwaggerOperation(Summary = "Equip Weapon to a Character",
                       Description = "Equips the specified weapon to the given character")]
    [SwaggerResponse(200, "Weapon Equipped", typeof(EquipmentResponse))]
    [SwaggerResponse(400, "Invalid ID or Weapon ID")]
    [SwaggerResponse(404, "Character or Weapon Not Found")]
    public async Task<ActionResult<EquipmentResponse>> EquipWeapon(
      [SwaggerParameter("Character identifier", Required = true)] Guid characterId,
      [SwaggerParameter("Weapon item identifier", Required = true)] int weaponId,
      [FromBody] [SwaggerRequestBody("Off-hand weapon details", Required = false)] EquipWeaponRequest request) {
      try {
        var character = await equipmentService.EquipWeaponAsync(characterId, weaponId, request.OffHand ?? false);
        var armorClass = equipmentRules.CalculateArmorClass(character);
        var weaponDamageModifier = equipmentRules.CalculateWeaponDamageModifier(character);
        return Ok(new EquipmentResponse {
          ArmorClass = armorClass,
          WeaponDamageModifier = weaponDamageModifier,
          Equipment = new EquipmentDetails {
            MainHandId = character.Equipment.MainHand?.Id,
            OffHandId = character.Equipment.OffHand?.Id,
            ArmorId = character.Equipment.Armor?.Id
          }
        });
      } catch (CharacterNotFoundException) {
        return NotFound(new {error = "CHARACTER_NOT_FOUND", message = "Character not found."});
      } catch (ItemNotFoundException) {
        return NotFound(new {error = "ITEM_NOT_FOUND", message = "The given item was not found."});
      } catch (EquipmentTypeMismatchException) {
        return NotFound(new {error = "WEAPON_NOT_FOUND", message = "The given item is not a weapon."});
      }
    }
  }
}
