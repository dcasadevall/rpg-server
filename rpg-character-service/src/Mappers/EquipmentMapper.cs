using RPGCharacterService.Dtos.Equipment;
using RPGCharacterService.Models.Characters;

namespace RPGCharacterService.Mappers {
    /// <summary>
    /// Static mapper class responsible for converting between equipment-related data models.
    /// Handles mapping between Character domain models and equipment DTOs.
    /// </summary>
    public static class EquipmentMapper {
        /// <summary>
        /// Converts a Character domain model to an EquipmentResponse DTO.
        /// Calculates derived properties such as armor class and weapon damage modifiers.
        /// </summary>
        /// <param name="character">The Character domain model containing equipment information.</param>
        /// <returns>An EquipmentResponse DTO containing the mapped equipment details and calculated values.</returns>
        /// <remarks>
        /// This method calculates:
        /// - Armor class based on equipped armor and character's dexterity
        /// - Weapon damage modifiers based on equipped weapons and character's ability scores
        /// </remarks>
        public static EquipmentResponse ToResponse(Character character) {
            var armorClass = character.CalculateArmorClass();
            var weaponDamageModifier = character.CalculateWeaponDamageModifier();

            return new EquipmentResponse {
                ArmorClass = armorClass,
                WeaponDamageModifier = weaponDamageModifier,
                Equipment = new EquipmentDetails {
                    MainHandId = character.Equipment.MainHand?.Id,
                    OffHandId = character.Equipment.OffHand?.Id,
                    ArmorId = character.Equipment.Armor?.Id
                }
            };
        }
    }
}
