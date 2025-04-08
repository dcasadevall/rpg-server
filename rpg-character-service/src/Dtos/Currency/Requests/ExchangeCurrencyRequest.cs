using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using RPGCharacterService.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace RPGCharacterService.Dtos.Currency.Requests {
  /// <summary>
  ///   Request object for exchanging one type of currency for another
  /// </summary>
  public record ExchangeCurrencyRequest {
    /// <summary>
    ///   The currency type to convert from
    /// </summary>
    [Required]
    [SwaggerSchema(Description = "The currency type to convert from (e.g., Gold, Silver, Copper)")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CurrencyType From { get; init; }

    /// <summary>
    ///   The currency type to convert to
    /// </summary>
    [Required]
    [SwaggerSchema(Description = "The currency type to convert to (e.g., Gold, Silver, Copper)")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CurrencyType To { get; init; }

    /// <summary>
    ///   The amount of the source currency to convert
    /// </summary>
    [Required]
    [Range(1, int.MaxValue)]
    [SwaggerSchema(Description = "The amount of the source currency to convert (must be positive)")]
    public int Amount { get; init; }
  }
}
