using RPGCharacterService.Models;

namespace RPGCharacterService.Models
{
    public enum CurrencyType
    {
        Copper,
        Silver,
        Electrum,
        Gold,
        Platinum
    }
    
    public class Wealth
    {
        public int Copper { get; private set; }
        public int Silver { get; private set; }
        public int Electrum { get; private set; }
        public int Gold { get; private set; }
        public int Platinum { get; private set; }

        public int GetCurrencyAmount(CurrencyType currencyType)
        {
            return currencyType switch
            {
                CurrencyType.Copper => Copper,
                CurrencyType.Silver => Silver,
                CurrencyType.Electrum => Electrum,
                CurrencyType.Gold => Gold,
                CurrencyType.Platinum => Platinum,
                _ => throw new ArgumentException($"Unknown currency type: {currencyType}")
            };
        }

        public void SetCurrencyAmount(CurrencyType currencyType, int amount)
        {
            switch (currencyType)
            {
                case CurrencyType.Copper:
                    Copper = amount;
                    break;
                case CurrencyType.Silver:
                    Silver = amount;
                    break;
                case CurrencyType.Electrum:
                    Electrum = amount;
                    break;
                case CurrencyType.Gold:
                    Gold = amount;
                    break;
                case CurrencyType.Platinum:
                    Platinum = amount;
                    break;
                default:
                    throw new ArgumentException($"Unknown currency type: {currencyType}");
            }
        }

        public void Merge(Wealth other)
        {
            foreach (var currencyType in Enum.GetValues<CurrencyType>())
            {
                SetCurrencyAmount(currencyType, GetCurrencyAmount(currencyType) + other.GetCurrencyAmount(currencyType));
            }
        }
    }
}
