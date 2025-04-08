using RPGCharacterService.Dtos.Currency.Requests;
using RPGCharacterService.Dtos.Currency.Responses;
using RPGCharacterService.Models;

namespace RPGCharacterService.Mappers {
  /// <summary>
  /// Static mapper class responsible for converting between currency-related data models.
  /// Handles mapping between Wealth domain models and currency DTOs.
  /// </summary>
  public static class CurrencyMapper {
    /// <summary>
    /// Converts a Wealth domain model to a CurrencyResponse DTO.
    /// </summary>
    /// <param name="wealth">The Wealth domain model containing currency amounts.</param>
    /// <returns>A CurrencyResponse DTO containing the mapped currency values.</returns>
    public static CurrencyResponse ToCurrencyResponse(Wealth wealth) {
      return new CurrencyResponse {
        Copper = wealth.Copper,
        Silver = wealth.Silver,
        Electrum = wealth.Electrum,
        Gold = wealth.Gold,
        Platinum = wealth.Platinum
      };
    }

    /// <summary>
    /// Converts a ModifyCurrencyRequest DTO to a dictionary of currency types and amounts.
    /// </summary>
    /// <param name="request">The ModifyCurrencyRequest containing currency modifications.</param>
    /// <returns>A dictionary mapping currency types to their respective amounts.</returns>
    /// <remarks>
    /// Null values in the request are converted to 0 in the resulting dictionary.
    /// </remarks>
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
