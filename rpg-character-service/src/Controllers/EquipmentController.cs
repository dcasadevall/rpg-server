using Microsoft.AspNetCore.Mvc;
using RPGCharacterService.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace RPGCharacterService.Controllers
{
    [ApiController]
    [Route("api/v1/characters/{characterId:guid}/equipment")]
    public class EquipmentController(ICharacterService characterService) : ControllerBase
    {
        [HttpPatch("armor/{armorId:int}")]
        [SwaggerOperation(Summary = "Equip Armor to a Character", 
                         Description = "Equips the specified armor to the given character")]
        [SwaggerResponse(200, "Armor Equipped", typeof(object))]
        [SwaggerResponse(400, "Invalid ID or Armor ID")]
        [SwaggerResponse(404, "Character or Armor Not Found")]
        public ActionResult<object> EquipArmor(
            [SwaggerParameter("Character identifier", Required = true)] Guid characterId, 
            [SwaggerParameter("Armor item identifier", Required = true)] int armorId)
        {
            try
            {
                // TODO: Implement armor equipping logic
                return Ok(new 
                { 
                    armorClass = 15,
                    attackModifier = "DEX",
                    equipment = new 
                    {
                        armor = armorId,
                        mainHand = 0,
                        offHand = 0
                    }
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = "CHARACTER_NOT_FOUND", message = "Character not found." });
            }
            catch (Exception)
            {
                return NotFound(new { error = "ARMOR_NOT_FOUND", message = "Armor not found." });
            }
        }
        
        [HttpPatch("weapons/{weaponId:int}")]
        [SwaggerOperation(Summary = "Equip Weapon to a Character", 
                         Description = "Equips the specified weapon to the given character")]
        [SwaggerResponse(200, "Weapon Equipped", typeof(object))]
        [SwaggerResponse(400, "Invalid ID or Weapon ID")]
        [SwaggerResponse(404, "Character or Weapon Not Found")]
        public ActionResult<object> EquipWeapon(
            [SwaggerParameter("Character identifier", Required = true)] Guid characterId, 
            [SwaggerParameter("Weapon item identifier", Required = true)] int weaponId, 
            [FromBody, SwaggerRequestBody("Off-hand weapon details", Required = false)] EquipWeaponRequest request)
        {
            try
            {
                var isOffHand = request?.OffHand ?? false;
                
                // TODO: Implement weapon equipping logic
                return Ok(new 
                { 
                    attackModifier = "STR",
                    armorClass = 12,
                    equipment = new 
                    {
                        mainHand = isOffHand ? 0 : weaponId,
                        offHand = isOffHand ? weaponId : 0
                    }
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = "CHARACTER_NOT_FOUND", message = "Character not found." });
            }
            catch (Exception)
            {
                return NotFound(new { error = "WEAPON_NOT_FOUND", message = "Weapon not found." });
            }
        }
    
        public class EquipWeaponRequest
        {
            [SwaggerSchema(Description = "Whether to equip the weapon in the off-hand slot")]
            public bool OffHand { get; set; }
        }
    }
} 
