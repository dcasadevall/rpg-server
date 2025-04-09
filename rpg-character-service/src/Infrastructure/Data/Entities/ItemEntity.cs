using RPGCharacterService.Models.Items;

namespace RPGCharacterService.Infrastructure.Data.Entities
{
    /// <summary>
    /// Database entity representing an item.
    /// </summary>
    public class ItemEntity
    {
        /// <summary>
        /// The unique identifier for this item.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the item.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The type of equipment (Armor, Weapon, Shield, or null for non-equipment items).
        /// </summary>
        public int? EquipmentType { get; set; }

        // Navigation properties

        /// <summary>
        /// The armor-specific stats for this item, if it is armor.
        /// </summary>
        public ArmorStatsEntity? ArmorStats { get; set; }

        /// <summary>
        /// The weapon-specific stats for this item, if it is a weapon.
        /// </summary>
        public WeaponStatsEntity? WeaponStats { get; set; }

        /// <summary>
        /// Gets the equipment type as an enum.
        /// </summary>
        public EquipmentType? GetEquipmentType() => EquipmentType.HasValue ? (EquipmentType)EquipmentType.Value : null;

        /// <summary>
        /// Sets the equipment type from an enum.
        /// </summary>
        public void SetEquipmentType(EquipmentType? equipmentType) => EquipmentType = equipmentType.HasValue ? (int)equipmentType.Value : null;
    }
}
