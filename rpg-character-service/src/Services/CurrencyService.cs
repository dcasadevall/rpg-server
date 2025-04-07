using RPGCharacterService.Models;
using RPGCharacterService.Models.Characters;
using RPGCharacterService.Persistence;

namespace RPGCharacterService.Services
{
    public interface ICurrencyService
    {
        Character GenerateInitialCurrency(Guid characterId);
        Character ModifyCurrencies(Guid characterId, Wealth currencies);
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

            if (character.InitFlags.HasFlag(CharacterInitializationFlags.CurrencyInitialized))
            {
                throw new InvalidOperationException("Currency already initialized for this character");
            }

            // Generate currency using dice rolls
            var goldAmount = diceService.Roll(DiceSides.Twenty, 1).Sum();
            var silverAmount = diceService.Roll(DiceSides.Twenty, 3).Sum();
            var copperAmount = diceService.Roll(DiceSides.Twelve, 5).Sum();

            character.Wealth.SetCurrencyAmount(CurrencyType.Gold, goldAmount);
            character.Wealth.SetCurrencyAmount(CurrencyType.Silver, silverAmount);
            character.Wealth.SetCurrencyAmount(CurrencyType.Copper, copperAmount);
            character.InitFlags |= CharacterInitializationFlags.CurrencyInitialized;
            
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
            
            if (!character.InitFlags.HasFlag(CharacterInitializationFlags.CurrencyInitialized))
            {
                throw new InvalidOperationException("Character's currency must be initialized before exchanging");
            }
            
            if (character.Wealth.GetCurrencyAmount(from) < amount)
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

            if (!exchangeRates.TryGetValue((from, to), out var rate))
            {
                throw new InvalidOperationException($"Cannot exchange {from} to {to}");
            }

            var fromAmount = character.Wealth.GetCurrencyAmount(from);
            var toAmount = character.Wealth.GetCurrencyAmount(to);
            
            character.Wealth.SetCurrencyAmount(from, fromAmount - amount);
            character.Wealth.SetCurrencyAmount(to, toAmount + (amount * rate));
            
            repository.Update(character);
            return character;
        }
        
        public Character ModifyCurrencies(Guid characterId, Wealth currencies)
        {
            var character = repository.GetById(characterId);
            if (character == null)
            {
                throw new KeyNotFoundException($"Character with ID {characterId} not found");
            }
            
            if (!character.InitFlags.HasFlag(CharacterInitializationFlags.CurrencyInitialized))
            {
                throw new InvalidOperationException("Character's currency must be initialized before modifying");
            }
            
            foreach (var currencyType in Enum.GetValues<CurrencyType>())
            {
                var currentAmount = character.Wealth.GetCurrencyAmount(currencyType);
                var newAmount = Math.Max(0, currentAmount + currencies.GetCurrencyAmount(currencyType));
                character.Wealth.SetCurrencyAmount(currencyType, newAmount);
            }
            
            repository.Update(character);
            return character;
        }
    }
} 
