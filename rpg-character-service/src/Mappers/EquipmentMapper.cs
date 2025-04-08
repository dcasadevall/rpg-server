using RPGCharacterService.Dtos.Equipment;
using RPGCharacterService.Models.Characters;

namespace RPGCharacterService.Mappers {
    public static class EquipmentMapper {
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
