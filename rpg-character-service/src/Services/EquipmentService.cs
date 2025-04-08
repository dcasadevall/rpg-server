using RPGCharacterService.Exceptions;
using RPGCharacterService.Models.Characters;
using RPGCharacterService.Models.Equipment;
using RPGCharacterService.Persistence;

namespace RPGCharacterService.Services
{
    public interface IEquipmentService
    {
        void EquipItem(Character character, uint itemId, EquipmentSlot slot);
        void UnequipItem(Character character, EquipmentSlot slot);
    }

    public class EquipmentService : IEquipmentService
    {
        private readonly IEquipmentRepository _equipmentRepository;

        public EquipmentService(IEquipmentRepository equipmentRepository)
        {
            _equipmentRepository = equipmentRepository;
        }

        public void EquipItem(Character character, uint itemId, EquipmentSlot slot)
        {
            var item = _equipmentRepository.GetById(itemId);
            if (item == null)
            {
                throw new InvalidEquipmentOperationException(itemId, slot);
            }

            // Validate equipment type and slot compatibility
            switch (slot)
            {
                case EquipmentSlot.MainHand:
                    if (item.Type != EquipmentType.Weapon)
                    {
                        throw new InvalidEquipmentOperationException(itemId, slot);
                    }
                    if (item.IsTwoHanded)
                    {
                        // Unequip offhand and armor if it's a shield
                        UnequipItem(character, EquipmentSlot.OffHand);
                    }
                    character.Equipment.MainHand = itemId;
                    break;

                case EquipmentSlot.OffHand:
                    if (item.Type != EquipmentType.Weapon && item.Type != EquipmentType.Shield)
                    {
                        throw new InvalidEquipmentOperationException(itemId, slot);
                    }
                    if (!item.CanBeOffHand)
                    {
                        throw new InvalidEquipmentOperationException(itemId, slot);
                    }
                    // Check if main hand is two-handed
                    if (character.Equipment.MainHand.HasValue)
                    {
                        var mainHandItem = _equipmentRepository.GetById(character.Equipment.MainHand.Value);
                        if (mainHandItem?.IsTwoHanded == true)
                        {
                            throw new InvalidEquipmentOperationException(itemId, slot);
                        }
                    }
                    character.Equipment.OffHand = itemId;
                    break;

                case EquipmentSlot.Armor:
                    if (item.Type != EquipmentType.Armor)
                    {
                        throw new InvalidEquipmentOperationException(itemId, slot);
                    }
                    character.Equipment.Armor = itemId;
                    break;

                default:
                    throw new InvalidEquipmentOperationException(itemId, slot);
            }

            // Update character's armor class
            UpdateCharacterArmorClass(character);
        }

        public void UnequipItem(Character character, EquipmentSlot slot)
        {
            switch (slot)
            {
                case EquipmentSlot.MainHand:
                    character.Equipment.MainHand = null;
                    break;
                case EquipmentSlot.OffHand:
                    character.Equipment.OffHand = null;
                    break;
                case EquipmentSlot.Armor:
                    character.Equipment.Armor = null;
                    break;
            }

            UpdateCharacterArmorClass(character);
        }

        private void UpdateCharacterArmorClass(Character character)
        {
            int baseArmorClass = 10; // Base AC without armor
            int armorBonus = 0;
            int shieldBonus = 0;

            // Calculate armor bonus
            if (character.Equipment.Armor.HasValue)
            {
                var armor = _equipmentRepository.GetById(character.Equipment.Armor.Value);
                if (armor != null)
                {
                    armorBonus = armor.ArmorClass;
                }
            }

            // Calculate shield bonus
            if (character.Equipment.OffHand.HasValue)
            {
                var offHand = _equipmentRepository.GetById(character.Equipment.OffHand.Value);
                if (offHand?.Type == EquipmentType.Shield)
                {
                    shieldBonus = offHand.ArmorClass;
                }
            }

            character.ArmorClass = baseArmorClass + armorBonus + shieldBonus;
        }
    }
} 
