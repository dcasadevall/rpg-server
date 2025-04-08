using RPGCharacterService.Models.Equipment;

namespace RPGCharacterService.Exceptions
{
    public class InvalidEquipmentOperationException(uint itemId, EquipmentSlot slot)
        : Exception($"Cannot equip item {itemId} in {slot} slot.")
    {
        public uint ItemId { get; } = itemId;
        public EquipmentSlot Slot { get; } = slot;
    }
} 
