using RPGCharacterService.Exceptions;
using RPGCharacterService.Models;
using RPGCharacterService.Services;

namespace RPGCharacterService.UnitTests.Services {
  public class DiceServiceTests {
    private readonly DiceService diceService = new();

    [Theory]
    [InlineData(DiceSides.Four, 1)]
    [InlineData(DiceSides.Six, 2)]
    [InlineData(DiceSides.Eight, 3)]
    [InlineData(DiceSides.Ten, 4)]
    [InlineData(DiceSides.Twelve, 5)]
    [InlineData(DiceSides.Twenty, 6)]
    public void Roll_WithValidInput_ShouldReturnCorrectNumberOfResults(DiceSides sides, int count) {
      // Act
      var result = diceService.Roll(sides, count);

      // Assert
      Assert.Equal(count, result.Length);
      Assert.All(result, roll => {
        Assert.True(roll >= 1 && roll <= (int)sides);
      });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Roll_WithInvalidCount_ShouldThrowInvalidDiceRollException(int invalidCount) {
      // Act & Assert
      Assert.Throws<InvalidDiceRollException>(() => diceService.Roll(DiceSides.Twenty, invalidCount));
    }

    [Theory]
    [InlineData(DiceSides.Four, 1000)] // Test with many rolls to verify distribution
    public void Roll_WithManyRolls_ShouldHaveReasonableDistribution(DiceSides sides, int count) {
      // Act
      var results = diceService.Roll(sides, count);

      // Assert
      var expectedAverage = ((int)sides + 1) / 2.0;
      var actualAverage = results.Average();
      var tolerance = 0.1 * expectedAverage; // Allow 10% deviation

      Assert.InRange(actualAverage, expectedAverage - tolerance, expectedAverage + tolerance);
    }
  }
}
