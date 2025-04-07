namespace RPGCharacterService.Dtos.Currency.Requests
{
    public record ModifyCurrencyRequest
    {
        public int? Gold { get; init; }
        public int? Silver { get; init; }
        public int? Bronze { get; init; }
        public int? Copper { get; init; }
        public int? Electrum { get; init; }
        public int? Platinum { get; init; }
    }
}
