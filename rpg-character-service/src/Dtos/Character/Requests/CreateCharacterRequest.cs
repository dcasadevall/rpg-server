using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace RPGCharacterService.Dtos.Character.Requests 
{
    /// <summary>
    /// Request object for creating a new character
    /// </summary>
    public record CreateCharacterRequest
    {
        /// <summary>
        /// The name of the character, between 3-15 characters, letters only
        /// </summary>
        [Required]
        [StringLength(15, MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-Z]+$")]
        [SwaggerSchema(Description = "The name of the character (3-15 characters, letters only)")]
        public string Name { get; init; }
        
        /// <summary>
        /// The character's race (e.g., Human, Elf, Dwarf)
        /// </summary>
        [Required]
        [StringLength(50)]
        [SwaggerSchema(Description = "The character's race (e.g., Human, Elf, Dwarf)")]
        public string Race { get; init; }
        
        /// <summary>
        /// The character's optional subrace (e.g., High, Mountain)
        /// </summary>
        [StringLength(50)]
        [SwaggerSchema(Description = "The character's optional subrace (e.g., High, Mountain)")]
        public string? Subrace { get; init; }
        
        /// <summary>
        /// The character's class (e.g., Fighter, Wizard, Cleric)
        /// </summary>
        [Required]
        [StringLength(50)]
        [SwaggerSchema(Description = "The character's class (e.g., Fighter, Wizard, Cleric)")]
        public string Class { get; init; }
    }
}
