namespace RPGCharacterService.Dtos.Currency.Requests
{
    public class ModifyCurrencyRequest
    {
        public int? Gold { get; set; }
        public int? Silver { get; set; }
        public int? Bronze { get; set; }
        public int? Copper { get; set; }
        public int? Electrum { get; set; }
        public int? Platinum { get; set; }
    }
}
