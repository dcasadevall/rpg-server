using Swashbuckle.AspNetCore.Annotations;

namespace RPGCharacterService.Dtos.Dice
{
    /// <summary>
    /// Response object for dice rolling operations
    /// </summary>
    public class RollDiceResponse
    {
        /// <summary>
        /// The results of each individual die roll
        /// </summary>
        [SwaggerSchema(Description = "The results of each individual die roll")]
        public IEnumerable<int> Results { get; init; }
    }
}
