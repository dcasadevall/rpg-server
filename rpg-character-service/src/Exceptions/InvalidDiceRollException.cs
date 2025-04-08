namespace RPGCharacterService.Exceptions {
  /// <summary>
  /// Exception thrown when attempting to roll dice with invalid parameters.
  /// This includes cases where the number of dice or sides is less than or equal to zero.
  /// </summary>
  public class InvalidDiceRollException(int diceCount, int diceSides)
    : Exception($"Invalid dice roll: {diceCount}d{diceSides}");
}
