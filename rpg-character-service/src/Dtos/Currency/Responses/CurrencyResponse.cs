using Swashbuckle.AspNetCore.Annotations;

namespace RPGCharacterService.Dtos.Currency.Responses
{
    /// <summary>
    /// Response object for currency-related operations
    /// </summary>
    public record CurrencyResponse
    {
        /// <summary>
        /// The amount of copper coins
        /// </summary>
        [SwaggerSchema(Description = "The amount of copper coins")]
        public int Copper { get; init; }

        /// <summary>
        /// The amount of silver coins
        /// </summary>
        [SwaggerSchema(Description = "The amount of silver coins")]
        public int Silver { get; init; }

        /// <summary>
        /// The amount of electrum coins
        /// </summary>
        [SwaggerSchema(Description = "The amount of electrum coins")]
        public int Electrum { get; init; }

        /// <summary>
        /// The amount of gold coins
        /// </summary>
        [SwaggerSchema(Description = "The amount of gold coins")]
        public int Gold { get; init; }

        /// <summary>
        /// The amount of platinum coins
        /// </summary>
        [SwaggerSchema(Description = "The amount of platinum coins")]
        public int Platinum { get; init; }
    }
}
