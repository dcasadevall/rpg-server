using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace RPGCharacterService.Dtos.Dice
{
    public class RollDiceRequest
    {
        [Required]
        [SwaggerParameter("Number of sides on the dice (allowed: 4, 6, 8, 10, 12, 20)", Required = true)]
        public int Sides { get; init; }

        [SwaggerParameter("Number of dice to roll (default 1)")]
        public int Count { get; init; } = 1;
    }
}
