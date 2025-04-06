using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using RPGCharacterService.Models;

namespace RPGCharacterService.Dtos.Currency.Requests
{
    public class ExchangeCurrencyRequest
    {
        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CurrencyType From { get; set; }
        
        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CurrencyType To { get; set; }
        
        [Required]
        public int Amount { get; set; }
    }
}
