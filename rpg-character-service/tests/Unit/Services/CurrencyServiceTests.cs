using RPGCharacterService.Exceptions.Currency;
using RPGCharacterService.Models;
using RPGCharacterService.Models.Characters;
using RPGCharacterService.Persistence.Characters;
using RPGCharacterService.Services;

namespace RPGCharacterService.UnitTests.Services {
  public class CurrencyServiceTests {
    private readonly Mock<ICharacterRepository> repositoryMock = new();
    private readonly Mock<IDiceService> diceServiceMock = new();
    private readonly CurrencyService currencyService;

    public CurrencyServiceTests() {
      currencyService = new CurrencyService(repositoryMock.Object, diceServiceMock.Object);
    }

    [Fact]
    public async Task GenerateInitialCurrencyAsync_WithUninitializedCharacter_ShouldInitializeCurrency() {
      // Arrange
      var character = new Character();
      repositoryMock.Setup(r => r.GetByIdAsync(character.Id))
                    .ReturnsAsync(character);
      diceServiceMock.Setup(d => d.Roll(DiceSides.Twenty, 1))
                      .Returns([10]);
      diceServiceMock.Setup(d => d.Roll(DiceSides.Twenty, 3))
                      .Returns([5, 10, 15]);
      diceServiceMock.Setup(d => d.Roll(DiceSides.Twelve, 5))
                      .Returns([3, 6, 9, 12, 15]);

      // Act
      var result = await currencyService.GenerateInitialCurrencyAsync(character.Id);

      // Assert
      Assert.Equal(10, result.Wealth.GetCurrencyAmount(CurrencyType.Gold));
      Assert.Equal(30, result.Wealth.GetCurrencyAmount(CurrencyType.Silver));
      Assert.Equal(45, result.Wealth.GetCurrencyAmount(CurrencyType.Copper));
      Assert.True(result.InitFlags.HasFlag(CharacterInitializationFlags.CurrencyInitialized));
      repositoryMock.Verify(r => r.UpdateAsync(It.Is<Character>(c => c.Id == character.Id)), Times.Once);
    }

    [Fact]
    public async Task GenerateInitialCurrencyAsync_WithInitializedCharacter_ShouldThrowCurrencyAlreadyInitializedException() {
      // Arrange
      var character = new Character {
        InitFlags = CharacterInitializationFlags.CurrencyInitialized
      };
      repositoryMock.Setup(r => r.GetByIdAsync(character.Id))
                    .ReturnsAsync(character);

      // Act & Assert
      await Assert.ThrowsAsync<CurrencyAlreadyInitializedException>(() =>
        currencyService.GenerateInitialCurrencyAsync(character.Id));
    }

    [Fact]
    public async Task ModifyCurrenciesAsync_WithValidChanges_ShouldUpdateCurrency() {
      // Arrange
      var character = new Character {
        InitFlags = CharacterInitializationFlags.CurrencyInitialized
      };
      character.Wealth.SetCurrencyAmount(CurrencyType.Gold, 10);
      character.Wealth.SetCurrencyAmount(CurrencyType.Silver, 20);
      character.Wealth.SetCurrencyAmount(CurrencyType.Copper, 30);
      repositoryMock.Setup(r => r.GetByIdAsync(character.Id))
                    .ReturnsAsync(character);
      var changes = new Dictionary<CurrencyType, int> {
        { CurrencyType.Gold, 5 },
        { CurrencyType.Silver, -10 },
        { CurrencyType.Copper, 15 }
      };

      // Act
      var result = await currencyService.ModifyCurrenciesAsync(character.Id, changes);

      // Assert
      Assert.Equal(15, result.Wealth.GetCurrencyAmount(CurrencyType.Gold));
      Assert.Equal(10, result.Wealth.GetCurrencyAmount(CurrencyType.Silver));
      Assert.Equal(45, result.Wealth.GetCurrencyAmount(CurrencyType.Copper));
      repositoryMock.Verify(r => r.UpdateAsync(It.Is<Character>(c => c.Id == character.Id)), Times.Once);
    }

    [Fact]
    public async Task ModifyCurrenciesAsync_WithInsufficientFunds_ShouldThrowNotEnoughCurrencyException() {
      // Arrange
      var character = new Character {
        InitFlags = CharacterInitializationFlags.CurrencyInitialized
      };
      character.Wealth.SetCurrencyAmount(CurrencyType.Gold, 10);
      repositoryMock.Setup(r => r.GetByIdAsync(character.Id))
                    .ReturnsAsync(character);
      var changes = new Dictionary<CurrencyType, int> {
        { CurrencyType.Gold, -15 }
      };

      // Act & Assert
      await Assert.ThrowsAsync<NotEnoughCurrencyException>(() =>
        currencyService.ModifyCurrenciesAsync(character.Id, changes));
    }

    [Fact]
    public async Task ExchangeCurrencyAsync_WithValidExchange_ShouldUpdateCurrencies() {
      // Arrange
      var character = new Character {
        InitFlags = CharacterInitializationFlags.CurrencyInitialized
      };
      character.Wealth.SetCurrencyAmount(CurrencyType.Gold, 10);
      character.Wealth.SetCurrencyAmount(CurrencyType.Silver, 0);
      repositoryMock.Setup(r => r.GetByIdAsync(character.Id))
                    .ReturnsAsync(character);

      // Act
      var result = await currencyService.ExchangeCurrencyAsync(character.Id, CurrencyType.Gold, CurrencyType.Silver, 1);

      // Assert
      Assert.Equal(9, result.Wealth.GetCurrencyAmount(CurrencyType.Gold));
      Assert.Equal(10, result.Wealth.GetCurrencyAmount(CurrencyType.Silver));
      repositoryMock.Verify(r => r.UpdateAsync(It.Is<Character>(c => c.Id == character.Id)), Times.Once);
    }

    [Fact]
    public async Task ExchangeCurrencyAsync_WithInsufficientFunds_ShouldThrowNotEnoughCurrencyException() {
      // Arrange
      var character = new Character {
        InitFlags = CharacterInitializationFlags.CurrencyInitialized
      };
      character.Wealth.SetCurrencyAmount(CurrencyType.Gold, 5);
      repositoryMock.Setup(r => r.GetByIdAsync(character.Id))
                    .ReturnsAsync(character);

      // Act & Assert
      await Assert.ThrowsAsync<NotEnoughCurrencyException>(() =>
        currencyService.ExchangeCurrencyAsync(character.Id, CurrencyType.Gold, CurrencyType.Silver, 10));
    }

    [Fact]
    public async Task ExchangeCurrencyAsync_WithInvalidExchange_ShouldThrowInvalidCurrencyExchangeException() {
      // Arrange
      var character = new Character {
        InitFlags = CharacterInitializationFlags.CurrencyInitialized
      };
      character.Wealth.SetCurrencyAmount(CurrencyType.Gold, 10);
      repositoryMock.Setup(r => r.GetByIdAsync(character.Id))
                    .ReturnsAsync(character);

      // Act & Assert
      await Assert.ThrowsAsync<InvalidCurrencyExchangeException>(() =>
        currencyService.ExchangeCurrencyAsync(character.Id, CurrencyType.Gold, CurrencyType.Platinum, 1));
    }
  }
}
