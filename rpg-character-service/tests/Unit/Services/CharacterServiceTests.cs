using RPGCharacterService.Dtos.Character;
using RPGCharacterService.Exceptions.Character;
using RPGCharacterService.Entities;
using RPGCharacterService.Entities.Characters;
using RPGCharacterService.Persistence.Characters;
using RPGCharacterService.Services;

namespace RPGCharacterService.UnitTests.Services {
  public class CharacterServiceTests {
    private readonly Mock<ICharacterRepository> repositoryMock;
    private readonly Mock<IDiceService> diceServiceMock;
    private readonly CharacterService characterService;

    public CharacterServiceTests() {
      repositoryMock = new Mock<ICharacterRepository>();
      diceServiceMock = new Mock<IDiceService>();
      characterService = new CharacterService(repositoryMock.Object, diceServiceMock.Object);
    }

    [Fact]
    public async Task GetAllCharactersAsync_ShouldReturnAllCharacters() {
      // Arrange
      var characters = new List<Character> {
        new() { Name = "Character 1" },
        new() { Name = "Character 2" }
      };
      repositoryMock.Setup(x => x.GetAllAsync())
        .ReturnsAsync(characters);

      // Act
      var result = await characterService.GetAllCharactersAsync();

      // Assert
      Assert.Equal(characters, result);
    }

    [Fact]
    public async Task GetCharacterAsync_WithValidId_ShouldReturnCharacter() {
      // Arrange
      var character = new Character { Name = "Test Character" };
      repositoryMock.Setup(x => x.GetByIdAsync(character.Id))
        .ReturnsAsync(character);

      // Act
      var result = await characterService.GetCharacterAsync(character.Id);

      // Assert
      Assert.Equal(character, result);
    }

    [Fact]
    public async Task GetCharacterAsync_WithInvalidId_ShouldThrowCharacterNotFoundException() {
      // Arrange
      var character = new Character { Name = "Test Character" };
      repositoryMock.Setup(x => x.GetByIdAsync(character.Id))
        .ThrowsAsync(new CharacterNotFoundException(character.Id));

      // Act & Assert
      await Assert.ThrowsAsync<CharacterNotFoundException>(() =>
        characterService.GetCharacterAsync(character.Id));
    }

    [Fact]
    public async Task CreateCharacterAsync_WithValidRequest_ShouldCreateCharacter() {
      // Arrange
      var request = new CreateCharacterRequest {
        Name = "Test Character",
        Race = "Human",
        Class = "Fighter"
      };

      var diceRolls = new[] { 6, 5, 4, 3 };
      diceServiceMock.Setup(x => x.Roll(DiceSides.Six, 4))
        .Returns(diceRolls);

      repositoryMock.Setup(x => x.GetByNameAsync(request.Name))
        .ReturnsAsync((Character?)null);

      // Act
      var result = await characterService.CreateCharacterAsync(request);

      // Assert
      Assert.Equal(request.Name, result.Name);
      Assert.Equal(request.Race, result.Race);
      Assert.Equal(request.Class, result.Class);
      Assert.Equal(1, result.Level);
      Assert.Equal(10 + (diceRolls.OrderByDescending(x => x).Take(3).Sum() - 10) / 2, result.HitPoints);
      repositoryMock.Verify(x => x.AddAsync(It.Is<Character>(c => c.Name == request.Name)), Times.Once);
    }

    [Fact]
    public async Task CreateCharacterAsync_WithExistingName_ShouldThrowCharacterAlreadyExistsException() {
      // Arrange
      var request = new CreateCharacterRequest {
        Name = "Test Character",
        Race = "Human",
        Class = "Fighter"
      };

      var existingCharacter = new Character { Name = request.Name };
      repositoryMock.Setup(x => x.GetByNameAsync(request.Name))
        .ReturnsAsync(existingCharacter);

      // Act & Assert
      await Assert.ThrowsAsync<CharacterAlreadyExistsException>(() =>
        characterService.CreateCharacterAsync(request));
    }

    [Fact]
    public async Task DeleteCharacterAsync_WithValidId_ShouldDeleteCharacter() {
      // Arrange
      var character = new Character { Name = "Test Character" };
      repositoryMock.Setup(x => x.DeleteAsync(character.Id))
        .Returns(Task.CompletedTask);

      // Act
      await characterService.DeleteCharacterAsync(character.Id);

      // Assert
      repositoryMock.Verify(x => x.DeleteAsync(character.Id), Times.Once);
    }

    [Fact]
    public async Task DeleteCharacterAsync_WithInvalidId_ShouldThrowCharacterNotFoundException() {
      // Arrange
      var character = new Character { Name = "Test Character" };
      repositoryMock.Setup(x => x.DeleteAsync(character.Id))
        .ThrowsAsync(new CharacterNotFoundException(character.Id));

      // Act & Assert
      await Assert.ThrowsAsync<CharacterNotFoundException>(() =>
        characterService.DeleteCharacterAsync(character.Id));
    }
  }
}
