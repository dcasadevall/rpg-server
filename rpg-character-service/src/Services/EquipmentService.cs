using RPGCharacterService.Exceptions;
using RPGCharacterService.Models.Characters;
using RPGCharacterService.Models.Equipment;
using RPGCharacterService.Persistence;
using RPGCharacterService.Persistence.Characters;

namespace RPGCharacterService.Services
{
    public interface IEquipmentService
    {
        Task<Character> EquipItemAsync(Guid characterId, uint itemId, EquipmentSlot slot);
        Task<Character> UnequipItemAsync(Guid characterId, EquipmentSlot slot);
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

        public async Task<Character> EquipItemAsync(Guid characterId, uint itemId, EquipmentSlot slot)
        {
            var character = await _characterRepository.GetByIdAsync(characterId);
            if (character == null)
            {
                throw new CharacterNotFoundException(characterId);
            }

            var item = await _equipmentRepository.GetByIdAsync(itemId);
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
                        var mainHandItem = await _equipmentRepository.GetByIdAsync(character.EquippedItems.MainHandId.Value);
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

            // Update character's armor class
            character.ArmorClass = CalculateArmorClass(character);
            
            await _characterRepository.UpdateAsync(character);
            return character;
        }

        public async Task<Character> UnequipItemAsync(Guid characterId, EquipmentSlot slot)
        {
            var character = await _characterRepository.GetByIdAsync(characterId);
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

            // Update character's armor class
            character.ArmorClass = CalculateArmorClass(character);
            
            await _characterRepository.UpdateAsync(character);
            return character;
        }

        public int CalculateArmorClass(Character character)
        {
            // Use the CharacterRules to calculate AC
            return _characterRules.CalculateArmorClass(character);
        }
    }
} 
