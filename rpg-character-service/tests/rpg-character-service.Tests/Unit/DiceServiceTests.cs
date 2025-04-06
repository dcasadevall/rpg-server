using RPGCharacterService.Services;

namespace character_manager.Tests.Unit 
{
    public class DiceServiceTests
    {
        private readonly DiceService diceService = new();

        [Theory]
        [InlineData(4, 1)]
        [InlineData(6, 1)]
        [InlineData(8, 1)]
        [InlineData(10, 1)]
        [InlineData(12, 1)]
        [InlineData(20, 1)]
        public void Roll_SingleDie_ReturnsValueInValidRange(int sides, int count)
        {
            // Act
            var result = diceService.Roll(sides, count);
            
            // Assert
            Assert.Single(result);
            Assert.InRange(result[0], 1, sides);
        }
        
        [Theory]
        [InlineData(6, 3)]
        [InlineData(20, 5)]
        public void Roll_MultipleDice_ReturnsCorrectNumberOfResults(int sides, int count)
        {
            // Act
            var result = diceService.Roll(sides, count);
            
            // Assert
            Assert.Equal(count, result.Length);
            Assert.All(result, r => Assert.InRange(r, 1, sides));
        }
        
        [Theory]
        [InlineData(3, 1)]
        [InlineData(5, 1)]
        [InlineData(100, 1)]
        public void Roll_InvalidSides_ThrowsArgumentException(int sides, int count)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => diceService.Roll(sides, count));
        }
        
        [Theory]
        [InlineData(6, 0)]
        [InlineData(6, -1)]
        [InlineData(6, 101)]
        public void Roll_InvalidCount_ThrowsArgumentException(int sides, int count)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => diceService.Roll(sides, count));
        }
        
        [Theory]
        [InlineData(6, 5)]
        public void Roll_MultipleDice_AllValuesInValidRange(int sides, int count)
        {
            // Act
            var result = diceService.Roll(sides, count);
            
            // Assert
            Assert.All(result, r => Assert.InRange(r, 1, sides));
        }
    }
} 
