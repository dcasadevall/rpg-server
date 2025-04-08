using RPGCharacterService.Exceptions;
using RPGCharacterService.Models.Characters;
using RPGCharacterService.Models.Equipment;
using RPGCharacterService.Persistence;

namespace RPGCharacterService.Services
{
    public interface IEquipmentService
    {
        Character EquipItem(Guid characterId, uint itemId, EquipmentSlot slot);
        Character UnequipItem(Guid characterId, EquipmentSlot slot);
        int CalculateArmorClass(Character character);
    }

    public class EquipmentService : IEquipmentService
    {
        private readonly IEquipmentRepository _equipmentRepository;
        private readonly ICharacterRepository _characterRepository;
        private readonly ICharacterRules _characterRules;

        public EquipmentService(
            IEquipmentRepository equipmentRepository, 
            ICharacterRepository characterRepository,
            ICharacterRules characterRules)
        {
            _equipmentRepository = equipmentRepository;
            _characterRepository = characterRepository;
            _characterRules = characterRules;
        }

        public Character EquipItem(Guid characterId, uint itemId, EquipmentSlot slot)
        {
            var character = _characterRepository.GetById(characterId);
            if (character == null)
            {
                throw new CharacterNotFoundException(characterId);
            }

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
                        // Unequip offhand
                        character.EquippedItems.OffHandId = null;
                        character.EquippedItems.OffHand = null;
                    }
                    character.EquippedItems.MainHandId = itemId;
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
                    if (character.EquippedItems.MainHandId.HasValue)
                    {
                        var mainHandItem = _equipmentRepository.GetById(character.EquippedItems.MainHandId.Value);
                        if (mainHandItem?.IsTwoHanded == true)
                        {
                            throw new InvalidEquipmentOperationException(itemId, slot);
                        }
                    }
                    character.EquippedItems.OffHandId = itemId;
                    break;

                case EquipmentSlot.Armor:
                    if (item.Type != EquipmentType.Armor)
                    {
                        throw new InvalidEquipmentOperationException(itemId, slot);
                    }
                    character.EquippedItems.ArmorId = itemId;
                    break;

                default:
                    throw new InvalidEquipmentOperationException(itemId, slot);
            }
            
            _characterRepository.Update(character);
            return character;
        }

        public Character UnequipItem(Guid characterId, EquipmentSlot slot)
        {
            var character = _characterRepository.GetById(characterId);
            if (character == null)
            {
                throw new CharacterNotFoundException(characterId);
            }

            switch (slot)
            {
                case EquipmentSlot.MainHand:
                    character.EquippedItems.MainHandId = null;
                    character.EquippedItems.MainHand = null;
                    break;
                case EquipmentSlot.OffHand:
                    character.EquippedItems.OffHandId = null;
                    character.EquippedItems.OffHand = null;
                    break;
                case EquipmentSlot.Armor:
                    character.EquippedItems.ArmorId = null;
                    character.EquippedItems.Armor = null;
                    break;
            }

            _characterRepository.Update(character);
            return character;
        }

        public int CalculateArmorClass(Character character)
        {
            // Use the CharacterRules to calculate AC
            return _characterRules.CalculateArmorClass(character);
        }
    }
} 
