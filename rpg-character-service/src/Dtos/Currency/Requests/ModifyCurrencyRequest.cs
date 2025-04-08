using Swashbuckle.AspNetCore.Annotations;

namespace RPGCharacterService.Dtos.Currency.Requests
{
    public record ModifyCurrencyRequest
    {
        [SwaggerParameter("Amount of gold coins in the character's wallet after modification")]
        public int? Gold { get; init; }
        public int? Silver { get; init; }
        public int? Copper { get; init; }
        public int? Electrum { get; init; }
        public int? Platinum { get; init; }
    }
}
