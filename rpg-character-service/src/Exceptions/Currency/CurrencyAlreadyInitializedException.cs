namespace RPGCharacterService.Exceptions.Currency
{
    public class CurrencyAlreadyInitializedException(Guid characterId)
        : Exception($"Currency has already been initialized for character with ID {characterId}.");
} 
