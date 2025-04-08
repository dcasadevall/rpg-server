using Swashbuckle.AspNetCore.Annotations;

namespace RPGCharacterService.Dtos.Currency.Requests
{
    /// <summary>
    /// Request object for modifying a character's currency
    /// </summary>
    public record ModifyCurrencyRequest
    {
        /// <summary>
        /// Amount of gold coins to add or subtract
        /// </summary>
        [SwaggerSchema(Description = "Amount of gold coins to add (positive) or subtract (negative)")]
        public int? Gold { get; init; }

        /// <summary>
        /// Amount of silver coins to add or subtract
        /// </summary>
        [SwaggerSchema(Description = "Amount of silver coins to add (positive) or subtract (negative)")]
        public int? Silver { get; init; }

        /// <summary>
        /// Amount of copper coins to add or subtract
        /// </summary>
        [SwaggerSchema(Description = "Amount of copper coins to add (positive) or subtract (negative)")]
        public int? Copper { get; init; }

        /// <summary>
        /// Amount of electrum coins to add or subtract
        /// </summary>
        [SwaggerSchema(Description = "Amount of electrum coins to add (positive) or subtract (negative)")]
        public int? Electrum { get; init; }

        /// <summary>
        /// Amount of platinum coins to add or subtract
        /// </summary>
        [SwaggerSchema(Description = "Amount of platinum coins to add (positive) or subtract (negative)")]
        public int? Platinum { get; init; }
    }
}
