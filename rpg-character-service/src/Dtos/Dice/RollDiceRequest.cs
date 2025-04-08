using System.ComponentModel.DataAnnotations;
using RPGCharacterService.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace RPGCharacterService.Dtos.Dice
{
    /// <summary>
    /// Request object for rolling dice
    /// </summary>
    public class RollDiceRequest
    {
        /// <summary>
        /// The number of sides on the dice
        /// </summary>
        [Required]
        [SwaggerSchema(Description = "The number of sides on the dice (e.g., 4, 6, 8, 10, 12, 20)")]
        public DiceSides Sides { get; set; }

        /// <summary>
        /// The number of dice to roll
        /// </summary>
        [Range(1, 100)]
        [SwaggerSchema(Description = "The number of dice to roll (1-100, default: 1)")]
        public int Count { get; set; } = 1;
    }
}
