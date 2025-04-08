using RPGCharacterService.Exceptions;
using RPGCharacterService.Exceptions.Character;
using RPGCharacterService.Exceptions.Currency;
using RPGCharacterService.Models;
using RPGCharacterService.Models.Characters;
using RPGCharacterService.Persistence;

namespace RPGCharacterService.Services
{
    public interface ICurrencyService
    {
        Character GenerateInitialCurrency(Guid characterId);
        Character ModifyCurrencies(Guid characterId, Dictionary<CurrencyType, int> currencyChanges);
        Character ExchangeCurrency(Guid characterId, CurrencyType from, CurrencyType to, int amount);
    }

    public class CurrencyService(ICharacterRepository repository, IDiceService diceService) : ICurrencyService
    {
        public Character GenerateInitialCurrency(Guid characterId)
        {
            var character = repository.GetById(characterId);
            if (character == null)
            {
                throw new CharacterNotFoundException(characterId);
            }

            if (character.InitFlags.HasFlag(CharacterInitializationFlags.CurrencyInitialized))
            {
                throw new CurrencyAlreadyInitializedException(characterId);
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
                throw new CharacterNotFoundException(characterId);
            }
            
            if (!character.InitFlags.HasFlag(CharacterInitializationFlags.CurrencyInitialized))
            {
                throw new CurrencyNotInitializedException(characterId);
            }
            
            if (character.Wealth.GetCurrencyAmount(from) < amount)
            {
                throw new NotEnoughCurrencyException(from, amount, character.Wealth.GetCurrencyAmount(from));
            }

            // Hardcoded exchange rates. If needed, inject this mapping or load from a config file.
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
                throw new InvalidCurrencyExchangeException(from, to);
            }

            var fromAmount = character.Wealth.GetCurrencyAmount(from);
            var toAmount = character.Wealth.GetCurrencyAmount(to);
            
            character.Wealth.SetCurrencyAmount(from, fromAmount - amount);
            character.Wealth.SetCurrencyAmount(to, toAmount + (amount * rate));
            
            repository.Update(character);
            return character;
        }
        
        public Character ModifyCurrencies(Guid characterId, Dictionary<CurrencyType, int> currencyChanges)
        {
            var character = repository.GetById(characterId);
            if (character == null)
            {
                throw new CharacterNotFoundException(characterId);
            }
            
            if (!character.InitFlags.HasFlag(CharacterInitializationFlags.CurrencyInitialized))
            {
                throw new CurrencyNotInitializedException(characterId);
            }
            
            foreach (var change in currencyChanges)
            {
                var currencyAmountAfter = character.Wealth.GetCurrencyAmount(change.Key) - change.Value;
                if (currencyAmountAfter < 0)
                {
                    throw new NotEnoughCurrencyException(change.Key, -change.Value, character.Wealth.GetCurrencyAmount(change.Key));
                }
                
                character.Wealth.SetCurrencyAmount(change.Key, currencyAmountAfter);
            }
            
            repository.Update(character);
            return character;
        }
    }
} 
