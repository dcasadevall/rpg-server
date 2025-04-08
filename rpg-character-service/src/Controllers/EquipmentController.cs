using Microsoft.AspNetCore.Mvc;
using RPGCharacterService.Dtos.Equipment;
using RPGCharacterService.Exceptions.Character;
using RPGCharacterService.Exceptions.Items;
using RPGCharacterService.Mappers;
using RPGCharacterService.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace RPGCharacterService.Controllers {
  /// <summary>
  /// Controller responsible for managing character equipment operations.
  /// Handles equipping and managing armor, weapons, and shields for characters.
  /// </summary>
  [ApiController]
  [ApiVersion("1.0")]
  [Route("api/v{version:apiVersion}/characters/{characterId:guid}/equipment")]
  public class EquipmentController(IEquipmentService equipmentService) : ControllerBase {
    /// <summary>
    /// Equips a piece of armor to a character.
    /// </summary>
    /// <param name="characterId">The unique identifier of the character.</param>
    /// <param name="armorId">The unique identifier of the armor item to equip.</param>
    /// <returns>The updated character with the new armor equipped.</returns>
    /// <remarks>
    /// The armor must exist in the system and be of type Armor.
    /// Only one piece of armor can be equipped at a time.
    /// </remarks>
    [HttpPatch("armor/{armorId:int}")]
    [SwaggerOperation(Summary = "Equip Armor to a Character",
                       Description = "Equips the specified armor to the given character")]
    [SwaggerResponse(200, "Armor Equipped", typeof(EquipmentResponse))]
    [SwaggerResponse(400, "Invalid Item Id")]
    [SwaggerResponse(404, "Character or Armor Not Found")]
    public async Task<ActionResult<EquipmentResponse>> EquipArmor(
      [SwaggerParameter("Character identifier", Required = true)] Guid characterId,
      [SwaggerParameter("Armor item identifier", Required = true)] int armorId) {
      try {
        var character = await equipmentService.EquipArmorAsync(characterId, armorId);
        return Ok(EquipmentMapper.ToResponse(character));
      } catch (CharacterNotFoundException) {
        return NotFound(new {error = "CHARACTER_NOT_FOUND", message = "Character not found."});
      } catch (ItemNotFoundException) {
        return NotFound(new {error = "ITEM_NOT_FOUND", message = "The given item was not found."});
      } catch (EquipmentTypeMismatchException) {
        return NotFound(new {error = "EQUIPMENT_MISMATCH", message = "The given item is not armor."});
      }
    }

    /// <summary>
    /// Equips a weapon to a character, either in the main hand or off-hand.
    /// </summary>
    /// <param name="characterId">The unique identifier of the character.</param>
    /// <param name="weaponId">The unique identifier of the weapon item to equip.</param>
    /// <param name="request">Optional request containing off-hand weapon details.</param>
    /// <returns>The updated character with the new weapon equipped.</returns>
    /// <remarks>
    /// The weapon must exist in the system and be of type Weapon.
    /// Characters can equip weapons in both main hand and off-hand slots.
    /// </remarks>
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
        return Ok(EquipmentMapper.ToResponse(character));
      } catch (CharacterNotFoundException) {
        return NotFound(new {error = "CHARACTER_NOT_FOUND", message = "Character not found."});
      } catch (ItemNotFoundException) {
        return NotFound(new {error = "ITEM_NOT_FOUND", message = "The given item was not found."});
      } catch (EquipmentTypeMismatchException) {
        return NotFound(new {error = "WEAPON_NOT_FOUND", message = "The given item is not a weapon."});
      }
    }

    /// <summary>
    /// Equips a shield to a character.
    /// </summary>
    /// <param name="characterId">The unique identifier of the character.</param>
    /// <param name="shieldId">The unique identifier of the shield item to equip.</param>
    /// <returns>The updated character with the new shield equipped.</returns>
    /// <remarks>
    /// The shield must exist in the system and be of type Shield.
    /// Only one shield can be equipped at a time.
    /// </remarks>
    [HttpPatch("shield/{shieldId:int}")]
    [SwaggerOperation(Summary = "Equip Shield to a Character",
                       Description = "Equips the specified shield to the given character")]
    [SwaggerResponse(200, "Shield Equipped", typeof(EquipmentResponse))]
    [SwaggerResponse(400, "Invalid Item Id")]
    [SwaggerResponse(404, "Character or Shield Not Found")]
    public async Task<ActionResult<EquipmentResponse>> EquipShield(
      [SwaggerParameter("Character identifier", Required = true)] Guid characterId,
      [SwaggerParameter("Shield item identifier", Required = true)] int shieldId) {
      try {
        var character = await equipmentService.EquipShieldAsync(characterId, shieldId);
        return Ok(EquipmentMapper.ToResponse(character));
      } catch (CharacterNotFoundException) {
        return NotFound(new {error = "CHARACTER_NOT_FOUND", message = "Character not found."});
      } catch (ItemNotFoundException) {
        return NotFound(new {error = "ITEM_NOT_FOUND", message = "The given item was not found."});
      } catch (EquipmentTypeMismatchException) {
        return NotFound(new {error = "SHIELD_NOT_FOUND", message = "The given item is not a shield."});
      }
    }
  }
}
