using RPGCharacterService.Entities.Characters;
using RPGCharacterService.Persistence;
using RPGCharacterService.Services;
using RPGCharacterService.Tests.Unit.Helpers;

namespace RPGCharacterService.UnitTests.Services {
  public class StatsServiceTests {
    private readonly Mock<ICharacterRepository> repositoryMock = new();
    private readonly StatsService statsService;

    public StatsServiceTests() {
      statsService = new StatsService(repositoryMock.Object);
    }

    [Fact]
    public async Task ModifyHitPointsAsync_WithPositiveDelta_ShouldIncreaseHitPoints() {
      // Arrange
      var character = CharacterFactory.CreateCharacter(
        abilityScores: new Dictionary<AbilityScore, int> {
          {AbilityScore.Constitution, 14} // +2 Con modifier
        },
        level: 1,
        hitPoints: 10
      );
      repositoryMock
        .Setup(r => r.GetByIdAsync(character.Id))
        .ReturnsAsync(character);

      // Act
      var result = await statsService.ModifyHitPointsAsync(character.Id, 5);

      // Assert
      Assert.Equal(12, character.CalculateMaxHitPoints());
      Assert.Equal(12, result.HitPoints);
      repositoryMock.Verify(r => r.UpdateAsync(It.Is<Character>(c => c.Id == character.Id)), Times.Once);
    }

    [Fact]
    public async Task ModifyHitPointsAsync_WithNegativeDelta_ShouldDecreaseHitPoints() {
      // Arrange
      var character = CharacterFactory.CreateCharacter(
        abilityScores: new Dictionary<AbilityScore, int> {
          {AbilityScore.Constitution, 14} // Modifier: 2
        },
        level: 1,
        hitPoints: 10
      );
      repositoryMock
        .Setup(r => r.GetByIdAsync(character.Id))
        .ReturnsAsync(character);

      // Act
      var result = await statsService.ModifyHitPointsAsync(character.Id, -3);

      // Assert
      Assert.Equal(12, result.CalculateMaxHitPoints());
      Assert.Equal(7, result.HitPoints);
      repositoryMock.Verify(r => r.UpdateAsync(It.Is<Character>(c => c.Id == character.Id)), Times.Once);
    }

    [Fact]
    public async Task ModifyHitPointsAsync_WithExcessiveHealing_ShouldClampToMaxHitPoints() {
      // Arrange
      var character = CharacterFactory.CreateCharacter(
        abilityScores: new Dictionary<AbilityScore, int> {
          {AbilityScore.Constitution, 14} // Modifier: 2
        },
        level: 1,
        hitPoints: 10
      );
      repositoryMock
        .Setup(r => r.GetByIdAsync(character.Id))
        .ReturnsAsync(character);

      // Act
      var result = await statsService.ModifyHitPointsAsync(character.Id, 20);

      // Assert
      Assert.Equal(12, result.CalculateMaxHitPoints());
      Assert.Equal(12, result.HitPoints); // Max HP = 10 + (2 * 1) = 12
      repositoryMock.Verify(r => r.UpdateAsync(It.Is<Character>(c => c.Id == character.Id)), Times.Once);
    }

    [Fact]
    public async Task ModifyHitPointsAsync_WithExcessiveDamage_ShouldClampToZero() {
      // Arrange
      var character = CharacterFactory.CreateCharacter(
        abilityScores: new Dictionary<AbilityScore, int> {
          {AbilityScore.Constitution, 14} // Modifier: 2
        },
        level: 1,
        hitPoints: 10
      );
      repositoryMock
        .Setup(r => r.GetByIdAsync(character.Id))
        .ReturnsAsync(character);

      // Act
      var result = await statsService.ModifyHitPointsAsync(character.Id, -15);

      // Assert
      Assert.Equal(12, result.CalculateMaxHitPoints());
      Assert.Equal(0, result.HitPoints);
      repositoryMock.Verify(r => r.UpdateAsync(It.Is<Character>(c => c.Id == character.Id)), Times.Once);
    }

    [Fact]
    public async Task ModifyHitPointsAsync_WithDifferentLevels_ShouldRespectMaxHitPoints() {
      // Arrange
      var character = CharacterFactory.CreateCharacter(
        abilityScores: new Dictionary<AbilityScore, int> {
          {AbilityScore.Constitution, 16} // Modifier: 3
        },
        level: 3,
        hitPoints: 15
      );
      repositoryMock
        .Setup(r => r.GetByIdAsync(character.Id))
        .ReturnsAsync(character);

      // Act
      var result = await statsService.ModifyHitPointsAsync(character.Id, 10);

      // Assert (Max hitPoints = 10 + (3 * 3) = 19)
      Assert.Equal(19, result.CalculateMaxHitPoints());
      Assert.Equal(19, result.HitPoints); // Max HP = 10 + (3 * 3) = 19
      repositoryMock.Verify(r => r.UpdateAsync(It.Is<Character>(c => c.Id == character.Id)), Times.Once);
    }
  }
}
