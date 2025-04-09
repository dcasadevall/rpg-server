using Moq;
using RPGCharacterService.Exceptions.Character;
using RPGCharacterService.Exceptions.Currency;
using RPGCharacterService.Entities;
using RPGCharacterService.Entities.Characters;
using RPGCharacterService.Persistence;
using RPGCharacterService.Services;

namespace RPGCharacterService.UnitTests.Services {
  public class CurrencyServiceTests {
    private readonly Mock<ICharacterRepository> repositoryMock = new();
    private readonly Mock<IDiceService> diceServiceMock = new();
    private readonly CurrencyService currencyService;

    public CurrencyServiceTests() {
      currencyService = new CurrencyService(repositoryMock.Object, diceServiceMock.Object);
    }

    // Helper to create a character with initialized currency
    private Character CreateCharacterWithCurrency(Dictionary<CurrencyType, int> initialAmounts) {
      var character = new Character {
        InitFlags = CharacterInitializationFlags.CurrencyInitialized
      };
      foreach (var kvp in initialAmounts) {
        character.Wealth.SetCurrencyAmount(kvp.Key, kvp.Value);
      }
      return character;
    }

    [Fact]
    public async Task GenerateInitialCurrencyAsync_WhenNotInitialized_ShouldSetCurrencyAndFlag() {
      // Arrange
      var character = new Character {InitFlags = CharacterInitializationFlags.None}; // Not initialized
      repositoryMock
        .Setup(r => r.GetByIdAsync(character.Id))
        .ReturnsAsync(character);
      diceServiceMock
        .Setup(d => d.Roll(DiceSides.Twenty, 1))
        .Returns([10]); // Example roll
      diceServiceMock
        .Setup(d => d.Roll(DiceSides.Twenty, 3))
        .Returns([5, 10, 15]); // Example roll
      diceServiceMock
        .Setup(d => d.Roll(DiceSides.Twelve, 5))
        .Returns([1, 2, 3, 4, 5]); // Example roll

      // Act
      var result = await currencyService.GenerateInitialCurrencyAsync(character.Id);

      // Assert
      Assert.True(result.InitFlags.HasFlag(CharacterInitializationFlags.CurrencyInitialized));
      Assert.Equal(10, result.Wealth.GetCurrencyAmount(CurrencyType.Gold)); // 10
      Assert.Equal(30, result.Wealth.GetCurrencyAmount(CurrencyType.Silver)); // 5+10+15
      Assert.Equal(15, result.Wealth.GetCurrencyAmount(CurrencyType.Copper)); // 1+2+3+4+5
      repositoryMock.Verify(r => r.UpdateAsync(It.Is<Character>(c => c.Id == character.Id &&
                                                                     c.InitFlags.HasFlag(CharacterInitializationFlags
                                                                       .CurrencyInitialized) &&
                                                                     c.Wealth.GetCurrencyAmount(CurrencyType.Gold) ==
                                                                     10)),
                            Times.Once);
    }

    [Fact]
    public async Task GenerateInitialCurrencyAsync_WhenAlreadyInitialized_ShouldThrowException() {
      // Arrange
      var character = CreateCharacterWithCurrency(new Dictionary<CurrencyType, int> {{CurrencyType.Gold, 1}});
      repositoryMock
        .Setup(r => r.GetByIdAsync(character.Id))
        .ReturnsAsync(character);

      // Act & Assert
      await Assert.ThrowsAsync<CurrencyAlreadyInitializedException>(() =>
                                                                      currencyService
                                                                        .GenerateInitialCurrencyAsync(character.Id));
      repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Character>()), Times.Never);
    }

    [Fact]
    public async Task ModifyCurrenciesAsync_WithSufficientFunds_ShouldUpdateAmounts() {
      // Arrange
      var initialCurrency = new Dictionary<CurrencyType, int> {
        {CurrencyType.Gold, 20},
        {CurrencyType.Silver, 50}
      };
      var character = CreateCharacterWithCurrency(initialCurrency);
      repositoryMock
        .Setup(r => r.GetByIdAsync(character.Id))
        .ReturnsAsync(character);

      var changes = new Dictionary<CurrencyType, int> {
        {CurrencyType.Gold, 5}, // Add 5 gold
        {CurrencyType.Silver, -10} // Subtract 10 silver
      };

      // Act
      var result = await currencyService.ModifyCurrenciesAsync(character.Id, changes);

      // Assert
      Assert.Equal(25, result.Wealth.GetCurrencyAmount(CurrencyType.Gold));
      Assert.Equal(40, result.Wealth.GetCurrencyAmount(CurrencyType.Silver));
      repositoryMock.Verify(r => r.UpdateAsync(It.Is<Character>(c => c.Id == character.Id &&
                                                                     c.Wealth.GetCurrencyAmount(CurrencyType.Gold) ==
                                                                     25 &&
                                                                     c.Wealth.GetCurrencyAmount(CurrencyType.Silver) ==
                                                                     40)),
                            Times.Once);
    }

    [Fact]
    public async Task ModifyCurrenciesAsync_WithInsufficientFunds_ShouldThrowException() {
      // Arrange
      var initialCurrency = new Dictionary<CurrencyType, int> {{CurrencyType.Gold, 5}};
      var character = CreateCharacterWithCurrency(initialCurrency);
      repositoryMock
        .Setup(r => r.GetByIdAsync(character.Id))
        .ReturnsAsync(character);

      var changes = new Dictionary<CurrencyType, int> {{CurrencyType.Gold, -10}}; // Try to subtract 10 gold

      // Act & Assert
      await Assert.ThrowsAsync<NotEnoughCurrencyException>(() =>
                                                             currencyService.ModifyCurrenciesAsync(character.Id,
                                                              changes));
      repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Character>()), Times.Never); // Ensure no update occurred
    }

    [Fact]
    public async Task ModifyCurrenciesAsync_WhenNotInitialized_ShouldThrowException() {
      // Arrange
      var character = new Character {InitFlags = CharacterInitializationFlags.None}; // Not initialized
      repositoryMock
        .Setup(r => r.GetByIdAsync(character.Id))
        .ReturnsAsync(character);
      var changes = new Dictionary<CurrencyType, int> {{CurrencyType.Gold, 10}};

      // Act & Assert
      await Assert.ThrowsAsync<CurrencyNotInitializedException>(() =>
                                                                  currencyService.ModifyCurrenciesAsync(character.Id,
                                                                   changes));
    }


    // --- ExchangeCurrencyAsync Tests --- Reverted Logic ---

    [Theory]
    // Standard Rates (From -> To)
    [InlineData(CurrencyType.Platinum, CurrencyType.Gold, 1, 1000, 0, 999, 10)] // 1 PP -> 10 GP (Rate: 10)
    [InlineData(CurrencyType.Gold, CurrencyType.Platinum, 10, 100, 0, 90, 1)] // 10 GP -> 1 PP (Rate: 0.1)
    [InlineData(CurrencyType.Gold, CurrencyType.Electrum, 1, 100, 0, 99, 2)] // 1 GP -> 2 EP (Rate: 2)
    [InlineData(CurrencyType.Electrum, CurrencyType.Gold, 2, 50, 0, 48, 1)] // 2 EP -> 1 GP (Rate: 0.5)
    [InlineData(CurrencyType.Electrum, CurrencyType.Silver, 1, 50, 0, 49, 5)] // 1 EP -> 5 SP (Rate: 5)
    [InlineData(CurrencyType.Silver, CurrencyType.Electrum, 5, 50, 0, 45, 1)] // 5 SP -> 1 EP (Rate: 0.2)
    [InlineData(CurrencyType.Silver, CurrencyType.Copper, 1, 50, 0, 49, 10)] // 1 SP -> 10 CP (Rate: 10)
    [InlineData(CurrencyType.Copper, CurrencyType.Silver, 10, 50, 0, 40, 1)] // 10 CP -> 1 SP (Rate: 0.1)
    [InlineData(CurrencyType.Gold,
                 CurrencyType.Silver,
                 1,
                 100,
                 5,
                 99,
                 15)] // 1 GP (cost) -> 10 SP (gain). Initial SP 5 -> final SP 15.
    public async Task ExchangeCurrencyAsync_WithValidExchange_ShouldUpdateCurrenciesCorrectly(
      CurrencyType fromCurrency,
      CurrencyType toCurrency,
      int amount, // Amount of 'from' currency to spend
      int initialFromAmount,
      int initialToAmount,
      int expectedFinalFromAmount,
      int expectedFinalToAmount) {
      // Arrange
      var initialCurrency = new Dictionary<CurrencyType, int> {
        {fromCurrency, initialFromAmount},
        {toCurrency, initialToAmount}
      };
      var character = CreateCharacterWithCurrency(initialCurrency);
      repositoryMock
        .Setup(r => r.GetByIdAsync(character.Id))
        .ReturnsAsync(character);

      // Act
      var result =
        await currencyService.ExchangeCurrencyAsync(character.Id, fromCurrency, toCurrency, amount);

      // Assert
      Assert.Equal(expectedFinalFromAmount, result.Wealth.GetCurrencyAmount(fromCurrency));
      Assert.Equal(expectedFinalToAmount, result.Wealth.GetCurrencyAmount(toCurrency));
      repositoryMock.Verify(r => r.UpdateAsync(It.Is<Character>(c => c.Id == character.Id)), Times.Once);
    }

    [Fact]
    public async Task ExchangeCurrencyAsync_WithInsufficientFromCurrency_ShouldThrowException() {
      // Arrange
      var initialCurrency = new Dictionary<CurrencyType, int> {{CurrencyType.Gold, 1}}; // Only 1 Gold
      var character = CreateCharacterWithCurrency(initialCurrency);
      repositoryMock
        .Setup(r => r.GetByIdAsync(character.Id))
        .ReturnsAsync(character);

      // Act & Assert
      await Assert.ThrowsAsync<NotEnoughCurrencyException>(() =>
                                                             currencyService
                                                               .ExchangeCurrencyAsync(character.Id,
                                                                CurrencyType.Gold,
                                                                CurrencyType.Silver,
                                                                2)); // Try to spend 2 Gold
      repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Character>()), Times.Never);
    }

    [Fact]
    public async Task ExchangeCurrencyAsync_WithSameCurrency_ShouldThrowException() {
      // Arrange
      var initialCurrency = new Dictionary<CurrencyType, int> {{CurrencyType.Gold, 10}};
      var character = CreateCharacterWithCurrency(initialCurrency);
      repositoryMock
        .Setup(r => r.GetByIdAsync(character.Id))
        .ReturnsAsync(character);

      // Act & Assert
      await Assert.ThrowsAsync<InvalidCurrencyExchangeException>(() =>
                                                                   currencyService
                                                                     .ExchangeCurrencyAsync(character.Id,
                                                                      CurrencyType.Gold,
                                                                      CurrencyType.Gold,
                                                                      1));
    }

    [Fact]
    public async Task ExchangeCurrencyAsync_WithNegativeAmount_ShouldThrowException() {
      // Arrange
      var initialCurrency = new Dictionary<CurrencyType, int> {{CurrencyType.Gold, 10}};
      var character = CreateCharacterWithCurrency(initialCurrency);
      repositoryMock
        .Setup(r => r.GetByIdAsync(character.Id))
        .ReturnsAsync(character);

      // Act & Assert
      await Assert.ThrowsAsync<InvalidCurrencyExchangeException>(() =>
                                                                   currencyService
                                                                     .ExchangeCurrencyAsync(character.Id,
                                                                      CurrencyType.Gold,
                                                                      CurrencyType.Silver,
                                                                      -1));
    }

    [Fact]
    public async Task ExchangeCurrencyAsync_WhenNotInitialized_ShouldThrowException() {
      // Arrange
      var character = new Character {InitFlags = CharacterInitializationFlags.None};
      repositoryMock
        .Setup(r => r.GetByIdAsync(character.Id))
        .ReturnsAsync(character);

      // Act & Assert
      await Assert.ThrowsAsync<CurrencyNotInitializedException>(() =>
                                                                  currencyService
                                                                    .ExchangeCurrencyAsync(character.Id,
                                                                     CurrencyType.Gold,
                                                                     CurrencyType.Silver,
                                                                     1));
    }

    [Fact]
    public async Task ExchangeCurrencyAsync_WhenCharacterNotFound_ShouldThrowException() {
      // Arrange
      var characterId = Guid.NewGuid();
      repositoryMock
        .Setup(r => r.GetByIdAsync(characterId))
        .ReturnsAsync((Character?) null);

      // Act & Assert
      await Assert.ThrowsAsync<CharacterNotFoundException>(() =>
                                                             currencyService
                                                               .ExchangeCurrencyAsync(characterId,
                                                                CurrencyType.Gold,
                                                                CurrencyType.Silver,
                                                                1));
    }

    [Fact]
    public async Task ExchangeCurrencyAsync_ToLargeExchange_ThrowsOverflowException() {
      // Arrange
      var initialCurrency = new Dictionary<CurrencyType, int> {{CurrencyType.Gold, int.MaxValue}};
      var character = CreateCharacterWithCurrency(initialCurrency);
      repositoryMock
        .Setup(r => r.GetByIdAsync(character.Id))
        .ReturnsAsync(character);

      // Act & Assert
      await Assert.ThrowsAsync<OverflowException>(() => currencyService.ExchangeCurrencyAsync(character.Id,
                                                   CurrencyType.Gold,
                                                   CurrencyType.Silver,
                                                   int.MaxValue));
    }

    [Fact]
    public async Task ExchangeCurrencyAsync_ResultingAmountTooLarge_ThrowsOverflowException() {
      // Arrange
      var initialCurrency = new Dictionary<CurrencyType, int> {
        {CurrencyType.Gold, int.MaxValue},
        {CurrencyType.Silver, 10}
      };
      var character = CreateCharacterWithCurrency(initialCurrency);
      repositoryMock
        .Setup(r => r.GetByIdAsync(character.Id))
        .ReturnsAsync(character);

      // Act & Assert
      await Assert.ThrowsAsync<OverflowException>(() => currencyService.ExchangeCurrencyAsync(character.Id,
                                                   CurrencyType.Silver,
                                                   CurrencyType.Gold,
                                                   10));
    }

    [Theory]
    [InlineData(CurrencyType.Copper, CurrencyType.Gold, 99)] // 99 CP -> 0 GP (Rate: 0.01)
    [InlineData(CurrencyType.Silver, CurrencyType.Gold, 8)] // 1 SP -> 0 GP (Rate: 0.1)
    [InlineData(CurrencyType.Copper, CurrencyType.Platinum, 999)] // 999 CP -> 0 PP (Rate: 0.001)
    [InlineData(CurrencyType.Silver, CurrencyType.Platinum, 8)] // 1 SP -> 0 PP (Rate: 0.1)
    [InlineData(CurrencyType.Gold, CurrencyType.Platinum, 9)] // 9 GP -> 0 PP (Rate: 0.1)
    [InlineData(CurrencyType.Electrum, CurrencyType.Gold, 1)] // 4 EP -> 0 GP (Rate: 0.5)
    public async Task ExchangeCurrencyAsync_WithoutEnoughFromAmount_ShouldThrowException(
      CurrencyType from,
      CurrencyType to,
      int amount) {
      // Arrange
      var initialCurrency = new Dictionary<CurrencyType, int> {{from, amount + 1}};
      var character = CreateCharacterWithCurrency(initialCurrency);
      repositoryMock
        .Setup(r => r.GetByIdAsync(character.Id))
        .ReturnsAsync(character);

      // Act & Assert
      await Assert.ThrowsAsync<InvalidCurrencyExchangeException>(() =>
                                                                   currencyService
                                                                     .ExchangeCurrencyAsync(character.Id,
                                                                      from,
                                                                      to,
                                                                      amount));
    }
  }
}
