using Microsoft.AspNetCore.Mvc;
using RPGCharacterService.Services;

namespace RPGCharacterService.Controllers
{
    [ApiController]
    [Route("api/v1/characters/{characterId:guid}/equipment")]
    public class EquipmentController(ICharacterService characterService) : ControllerBase
    {
        [HttpPatch("armor/{armorId:int}")]
        public ActionResult<object> EquipArmor(Guid characterId, int armorId)
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
        public ActionResult<object> EquipWeapon(Guid characterId, int weaponId, [FromBody] EquipWeaponRequest request)
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
            public bool OffHand { get; set; }
        }
    }
} 
