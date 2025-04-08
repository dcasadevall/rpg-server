using Swashbuckle.AspNetCore.Annotations;

namespace RPGCharacterService.Dtos.Dice
{
    public class RollDiceResponse
    {
        [SwaggerParameter("Results of the dice rolls")]
        public IEnumerable<int> Results { get; init; }
    }
}
