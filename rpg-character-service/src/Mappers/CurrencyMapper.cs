using RPGCharacterService.Dtos.Currency.Requests;
using RPGCharacterService.Dtos.Currency.Responses;
using RPGCharacterService.Models;

namespace RPGCharacterService.Mappers {
  public static class CurrencyMapper {
    public static CurrencyResponse ToCurrencyResponse(Wealth wealth) {
      return new CurrencyResponse {
        Copper = wealth.Copper,
        Silver = wealth.Silver,
        Electrum = wealth.Electrum,
        Gold = wealth.Gold,
        Platinum = wealth.Platinum
      };
    }

    public static Dictionary<CurrencyType, int> ToDictionary(ModifyCurrencyRequest request) {
      return new Dictionary<CurrencyType, int> {
        {CurrencyType.Copper, request.Copper ?? 0},
        {CurrencyType.Silver, request.Silver ?? 0},
        {CurrencyType.Electrum, request.Electrum ?? 0},
        {CurrencyType.Gold, request.Gold ?? 0},
        {CurrencyType.Platinum, request.Platinum ?? 0}
      };
    }
  }
}
