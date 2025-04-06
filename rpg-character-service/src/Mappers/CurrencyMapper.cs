using RPGCharacterService.Dtos.Currency.Requests;
using RPGCharacterService.Dtos.Currency.Responses;
using RPGCharacterService.Models;

namespace RPGCharacterService.Mappers
{
    public static class CurrencyMapper
    {
        public static CurrencyResponse ToResponse(Dictionary<CurrencyType, int> currencies)
        {
            return new CurrencyResponse
            {
                Copper = currencies.GetValueOrDefault(CurrencyType.Copper, 0),
                Silver = currencies.GetValueOrDefault(CurrencyType.Silver, 0),
                Electrum = currencies.GetValueOrDefault(CurrencyType.Electrum, 0),
                Gold = currencies.GetValueOrDefault(CurrencyType.Gold, 0),
                Platinum = currencies.GetValueOrDefault(CurrencyType.Platinum, 0)
            };
        }
        
        public static Dictionary<CurrencyType, int> ToDictionary(CurrencyResponse response)
        {
            return new Dictionary<CurrencyType, int>
            {
                { CurrencyType.Copper, response.Copper },
                { CurrencyType.Silver, response.Silver },
                { CurrencyType.Electrum, response.Electrum },
                { CurrencyType.Gold, response.Gold },
                { CurrencyType.Platinum, response.Platinum }
            };
        }
        
        public static Dictionary<CurrencyType, int> ToCurrencyDictionary(ModifyCurrencyRequest request)
        {
            var result = new Dictionary<CurrencyType, int>();

            if (request.Gold.HasValue)
            {
                result[CurrencyType.Gold] = request.Gold.Value;
            }
            if (request.Silver.HasValue)
            {
                result[CurrencyType.Silver] = request.Silver.Value;
            }
            if (request.Bronze.HasValue)
            {
                result[CurrencyType.Bronze] = request.Bronze.Value;
            }
            if (request.Copper.HasValue)
            {
                result[CurrencyType.Copper] = request.Copper.Value;
            }
            if (request.Electrum.HasValue)
            {
                result[CurrencyType.Electrum] = request.Electrum.Value;
            }
            if (request.Platinum.HasValue)
            {
                result[CurrencyType.Platinum] = request.Platinum.Value;
            }

            return result;
        }
    }
}
