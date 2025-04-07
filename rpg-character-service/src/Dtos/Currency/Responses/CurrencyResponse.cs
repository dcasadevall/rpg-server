namespace RPGCharacterService.Dtos.Currency.Responses
{
    public record CurrencyResponse
    {
        public int Copper { get; init; }
        public int Silver { get; init; }
        public int Electrum { get; init; }
        public int Gold { get; init; }
        public int Platinum { get; init; }
    }
}
