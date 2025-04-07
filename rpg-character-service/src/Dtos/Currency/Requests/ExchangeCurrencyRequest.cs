using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using RPGCharacterService.Models;

namespace RPGCharacterService.Dtos.Currency.Requests
{
    public record ExchangeCurrencyRequest
    {
        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CurrencyType From { get; init; }
        
        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CurrencyType To { get; init; }
        
        [Required]
        public int Amount { get; init; }
    }
}
