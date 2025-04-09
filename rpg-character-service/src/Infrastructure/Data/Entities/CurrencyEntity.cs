namespace RPGCharacterService.Infrastructure.Data.Entities
{
    /// <summary>
    /// Database entity representing a character's currency.
    /// </summary>
    public class CurrencyEntity
    {
        /// <summary>
        /// The unique identifier for this currency record.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The ID of the character this currency belongs to.
        /// </summary>
        public Guid CharacterId { get; set; }

        /// <summary>
        /// The amount of copper pieces.
        /// </summary>
        public int Copper { get; set; }

        /// <summary>
        /// The amount of silver pieces.
        /// </summary>
        public int Silver { get; set; }

        /// <summary>
        /// The amount of electrum pieces.
        /// </summary>
        public int Electrum { get; set; }

        /// <summary>
        /// The amount of gold pieces.
        /// </summary>
        public int Gold { get; set; }

        /// <summary>
        /// The amount of platinum pieces.
        /// </summary>
        public int Platinum { get; set; }

        // Navigation property

        /// <summary>
        /// The character this currency belongs to.
        /// </summary>
        public CharacterEntity Character { get; set; } = null!;
    }
}
