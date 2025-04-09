using RPGCharacterService.Models.Characters;

namespace RPGCharacterService.Infrastructure.Data.Entities
{
    /// <summary>
    /// Database entity representing a character.
    /// </summary>
    public class CharacterEntity
    {
        /// <summary>
        /// The unique identifier for this character.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The character's name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The character's race (e.g., Human, Elf, Dwarf).
        /// </summary>
        public string Race { get; set; } = string.Empty;

        /// <summary>
        /// The character's subrace (e.g., High, Mountain, Deep, etc.).
        /// </summary>
        public string Subrace { get; set; } = string.Empty;

        /// <summary>
        /// The character's class (e.g., Fighter, Wizard, Rogue).
        /// </summary>
        public string Class { get; set; } = string.Empty;

        /// <summary>
        /// The character's current hit points.
        /// </summary>
        public int HitPoints { get; set; }

        /// <summary>
        /// The character's level, which affects proficiency bonus and other level-dependent calculations.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// The initialization flags that track which aspects of the character have been initialized.
        /// </summary>
        public int InitFlags { get; set; }

        /// <summary>
        /// The date and time when the character was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The date and time when the character was last updated.
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        // Navigation properties

        /// <summary>
        /// The character's ability scores.
        /// </summary>
        public ICollection<AbilityScoreEntity> AbilityScores { get; set; } = new List<AbilityScoreEntity>();

        /// <summary>
        /// The character's currency.
        /// </summary>
        public CurrencyEntity? Currency { get; set; }

        /// <summary>
        /// The character's equipment.
        /// </summary>
        public CharacterEquipmentEntity? Equipment { get; set; }
    }
}
