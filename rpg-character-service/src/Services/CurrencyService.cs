using RPGCharacterService.Models;
using RPGCharacterService.Persistence;

namespace RPGCharacterService.Services
{
    public interface ICurrencyService
    {
        Character GenerateInitialCurrency(Guid characterId);
        Character ModifyCurrencies(Guid characterId, Dictionary<CurrencyType, int> currencies);
        Character ExchangeCurrency(Guid characterId, CurrencyType from, CurrencyType to, int amount);
    }

    public class CurrencyService(ICharacterRepository repository, IDiceService diceService) : ICurrencyService
    {
        public Character GenerateInitialCurrency(Guid characterId)
        {
            var character = repository.GetById(characterId);
            if (character == null)
            {
                throw new KeyNotFoundException($"Character with ID {characterId} not found");
            }

            if (character.Currencies is {Count: > 0})
            {
                throw new InvalidOperationException("Currency already initialized for this character");
            }

            // Generate currency using dice rolls
            var goldAmount = diceService.Roll(20, 1).Sum();
            var silverAmount = diceService.Roll(20, 3).Sum();
            var copperAmount = diceService.Roll(20, 5).Sum();

            character.Currencies = new Dictionary<CurrencyType, int>
            {
                { CurrencyType.Gold, goldAmount },
                { CurrencyType.Silver, silverAmount },
                { CurrencyType.Copper, copperAmount },
                { CurrencyType.Electrum, 0 },
                { CurrencyType.Platinum, 0 }
            };
            
            repository.Update(character);

            return character;
        }

        public Character ExchangeCurrency(Guid characterId, CurrencyType from, CurrencyType to, int amount)
        {
            var character = repository.GetById(characterId);
            if (character == null)
            {
                throw new KeyNotFoundException($"Character with ID {characterId} not found");
            }
            
            if (!character.Currencies.ContainsKey(from) || !character.Currencies.ContainsKey(to))
            {
                throw new ArgumentException("Invalid currency type");
            }

            if (character.Currencies[from] < amount)
            {
                throw new InvalidOperationException($"Not enough {from} currency");
            }

            // Exchange rates (simplified for this implementation)
            var exchangeRates = new Dictionary<(CurrencyType, CurrencyType), int>
            {
                { (CurrencyType.Copper, CurrencyType.Silver), 10 },
                { (CurrencyType.Silver, CurrencyType.Gold), 10 },
                { (CurrencyType.Gold, CurrencyType.Platinum), 10 },
                { (CurrencyType.Silver, CurrencyType.Copper), 1 },
                { (CurrencyType.Gold, CurrencyType.Silver), 1 },
                { (CurrencyType.Platinum, CurrencyType.Gold), 1 }
            };

            if (!exchangeRates.TryGetValue((from, to), out int rate))
            {
                throw new InvalidOperationException($"Cannot exchange {from} to {to}");
            }

            character.Currencies[from] -= amount;
            character.Currencies[to] += amount * rate;
            
            repository.Update(character);
            return character;
        }
        
        public Character ModifyCurrencies(Guid characterId, Dictionary<CurrencyType, int> currencies)
        {
            var character = repository.GetById(characterId);
            if (character == null)
            {
                throw new KeyNotFoundException($"Character with ID {characterId} not found");
            }
            
            foreach (var currency in currencies)
            {
                character.Currencies[currency.Key] = Math.Max(0, character.Currencies[currency.Key] + currency.Value);
            }
            
            repository.Update(character);
            return character;
        }
    }
} 
