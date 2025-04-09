using RPGCharacterService.Models.Items;

namespace RPGCharacterService.Infrastructure.Data.Entities
{
    /// <summary>
    /// Database entity representing armor-specific statistics.
    /// </summary>
    public class ArmorStatsEntity
    {
        /// <summary>
        /// The unique identifier for these armor stats.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The ID of the item these armor stats belong to.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// The base armor class value for this armor.
        /// </summary>
        public int BaseArmorClass { get; set; }

        /// <summary>
        /// The type of armor (Light, Medium, Heavy, or None).
        /// </summary>
        public int ArmorType { get; set; }

        // Navigation property

        /// <summary>
        /// The item these armor stats belong to.
        /// </summary>
        public ItemEntity Item { get; set; } = null!;

        /// <summary>
        /// Gets the armor type as an enum.
        /// </summary>
        public ArmorType GetArmorType() => (ArmorType)ArmorType;

        /// <summary>
        /// Sets the armor type from an enum.
        /// </summary>
        public void SetArmorType(ArmorType armorType) => ArmorType = (int)armorType;
    }
}
