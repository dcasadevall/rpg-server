using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace RPGCharacterService.Dtos.Stats.Requests
{
    /// <summary>
    /// Request object for updating a character's hit points
    /// </summary>
    public class HitPointUpdateRequest
    {
        /// <summary>
        /// The amount to add or subtract from the character's current hit points
        /// </summary>
        [Required]
        [SwaggerSchema(Description = "The amount to change the hit points by; use positive values to heal, negative to damage")]
        public int Delta { get; set; }
    }
} 
