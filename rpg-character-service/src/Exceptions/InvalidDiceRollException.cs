namespace RPGCharacterService.Exceptions
{
    public class InvalidDiceRollException(int diceCount, int diceSides)
        : Exception($"Invalid dice roll: {diceCount}d{diceSides}");
} 
