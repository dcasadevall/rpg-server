using RPGCharacterService.Dtos.Currency.Requests;
using RPGCharacterService.Dtos.Currency.Responses;
using RPGCharacterService.Models;

namespace RPGCharacterService.Mappers
{
    public static class CurrencyMapper
    {
        public static CurrencyResponse ToCurrencyResponse(Wealth wealth)
        {
            return new CurrencyResponse
            {
                Copper = wealth.Copper,
                Silver = wealth.Silver,
                Electrum = wealth.Electrum,
                Gold = wealth.Gold,
                Platinum = wealth.Platinum
            };
        }
        
        public static Wealth ToWealth(CurrencyResponse response)
        {
            var wealth = new Wealth();
            wealth.SetCurrencyAmount(CurrencyType.Copper, response.Copper);
            wealth.SetCurrencyAmount(CurrencyType.Silver, response.Silver);
            wealth.SetCurrencyAmount(CurrencyType.Electrum, response.Electrum);
            wealth.SetCurrencyAmount(CurrencyType.Gold, response.Gold);
            wealth.SetCurrencyAmount(CurrencyType.Platinum, response.Platinum);
            return wealth;
        }
        
        public static Wealth ToWealth(ModifyCurrencyRequest request)
        {
            var wealth = new Wealth();
            
            if (request.Gold.HasValue)
            {
                wealth.SetCurrencyAmount(CurrencyType.Gold, request.Gold.Value);
            }
            if (request.Silver.HasValue)
            {
                wealth.SetCurrencyAmount(CurrencyType.Silver, request.Silver.Value);
            }
            if (request.Copper.HasValue)
            {
                wealth.SetCurrencyAmount(CurrencyType.Copper, request.Copper.Value);
            }
            if (request.Electrum.HasValue)
            {
                wealth.SetCurrencyAmount(CurrencyType.Electrum, request.Electrum.Value);
            }
            if (request.Platinum.HasValue)
            {
                wealth.SetCurrencyAmount(CurrencyType.Platinum, request.Platinum.Value);
            }

            return wealth;
        }
    }
}
