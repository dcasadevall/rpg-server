using RPGCharacterService.Exceptions;
using RPGCharacterService.Models;

namespace RPGCharacterService.Services {
  /// <summary>
  /// Defines the interface for dice rolling services.
  /// </summary>
  public interface IDiceService {
    /// <summary>
    /// Rolls a specified number of dice with a given number of sides.
    /// </summary>
    /// <param name="sides">The number of sides on each die.</param>
    /// <param name="count">The number of dice to roll.</param>
    /// <returns>An array of integers representing the results of each die roll.</returns>
    /// <exception cref="InvalidDiceRollException">Thrown when the number of dice or sides is invalid.</exception>
    int[] Roll(DiceSides sides, int count);
  }

  /// <summary>
  /// Provides functionality for rolling dice.
  /// This service is used for various game mechanics that require random number generation.
  /// </summary>
  public class DiceService : IDiceService {
    private readonly Random random = new();

    /// <summary>
    /// Rolls a specified number of dice with a given number of sides.
    /// </summary>
    /// <param name="sides">The number of sides on each die.</param>
    /// <param name="count">The number of dice to roll.</param>
    /// <returns>An array of integers representing the results of each die roll.</returns>
    /// <exception cref="InvalidDiceRollException">Thrown when the number of dice or sides is invalid.</exception>
    public int[] Roll(DiceSides sides, int count) {
      if (count <= 0) {
        throw new InvalidDiceRollException(count, (int) sides);
      }

      if (sides <= 0) {
        throw new InvalidDiceRollException(count, (int) sides);
      }

      return Enumerable
             .Range(0, count)
             .Select(_ => random.Next(1, (int) sides + 1))
             .ToArray();
    }
  }
}
