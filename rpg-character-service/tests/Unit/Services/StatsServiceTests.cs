using RPGCharacterService.Models.Characters;
using RPGCharacterService.Persistence.Characters;
using RPGCharacterService.Services;

namespace RPGCharacterService.UnitTests.Services {
  public class StatsServiceTests {
    private readonly Mock<ICharacterRepository> _repositoryMock = new();
    private readonly StatsService _statsService;

    public StatsServiceTests() {
      _statsService = new StatsService(_repositoryMock.Object);
    }

    [Fact]
    public async Task ModifyHitPointsAsync_WithPositiveDelta_ShouldIncreaseHitPoints() {
      // Arrange
      var character = new Character {
        AbilityScores = new Dictionary<AbilityScore, int> {
          {AbilityScore.Constitution, 14} // Modifier: 2
        },
        Level = 1,
        HitPoints = 10
      };
      _repositoryMock.Setup(r => r.GetByIdOrThrowAsync(character.Id))
                    .ReturnsAsync(character);

      // Act
      var result = await _statsService.ModifyHitPointsAsync(character.Id, 5);

      // Assert
      Assert.Equal(15, result.HitPoints);
      _repositoryMock.Verify(r => r.UpdateAsync(It.Is<Character>(c => c.Id == character.Id)), Times.Once);
    }

    [Fact]
    public async Task ModifyHitPointsAsync_WithNegativeDelta_ShouldDecreaseHitPoints() {
      // Arrange
      var character = new Character {
        AbilityScores = new Dictionary<AbilityScore, int> {
          {AbilityScore.Constitution, 14} // Modifier: 2
        },
        Level = 1,
        HitPoints = 10
      };
      _repositoryMock.Setup(r => r.GetByIdOrThrowAsync(character.Id))
                    .ReturnsAsync(character);

      // Act
      var result = await _statsService.ModifyHitPointsAsync(character.Id, -3);

      // Assert
      Assert.Equal(7, result.HitPoints);
      _repositoryMock.Verify(r => r.UpdateAsync(It.Is<Character>(c => c.Id == character.Id)), Times.Once);
    }

    [Fact]
    public async Task ModifyHitPointsAsync_WithExcessiveHealing_ShouldClampToMaxHitPoints() {
      // Arrange
      var character = new Character {
        AbilityScores = new Dictionary<AbilityScore, int> {
          {AbilityScore.Constitution, 14} // Modifier: 2
        },
        Level = 1,
        HitPoints = 10
      };
      _repositoryMock.Setup(r => r.GetByIdOrThrowAsync(character.Id))
                    .ReturnsAsync(character);

      // Act
      var result = await _statsService.ModifyHitPointsAsync(character.Id, 20);

      // Assert
      Assert.Equal(12, result.HitPoints); // Max HP = 10 + (2 * 1) = 12
      _repositoryMock.Verify(r => r.UpdateAsync(It.Is<Character>(c => c.Id == character.Id)), Times.Once);
    }

    [Fact]
    public async Task ModifyHitPointsAsync_WithExcessiveDamage_ShouldClampToZero() {
      // Arrange
      var character = new Character {
        AbilityScores = new Dictionary<AbilityScore, int> {
          {AbilityScore.Constitution, 14} // Modifier: 2
        },
        Level = 1,
        HitPoints = 10
      };
      _repositoryMock.Setup(r => r.GetByIdOrThrowAsync(character.Id))
                    .ReturnsAsync(character);

      // Act
      var result = await _statsService.ModifyHitPointsAsync(character.Id, -15);

      // Assert
      Assert.Equal(0, result.HitPoints);
      _repositoryMock.Verify(r => r.UpdateAsync(It.Is<Character>(c => c.Id == character.Id)), Times.Once);
    }

    [Fact]
    public async Task ModifyHitPointsAsync_WithDifferentLevels_ShouldRespectMaxHitPoints() {
      // Arrange
      var character = new Character {
        AbilityScores = new Dictionary<AbilityScore, int> {
          {AbilityScore.Constitution, 16} // Modifier: 3
        },
        Level = 3,
        HitPoints = 15
      };
      _repositoryMock.Setup(r => r.GetByIdOrThrowAsync(character.Id))
                    .ReturnsAsync(character);

      // Act
      var result = await _statsService.ModifyHitPointsAsync(character.Id, 10);

      // Assert
      Assert.Equal(19, result.HitPoints); // Max HP = 10 + (3 * 3) = 19
      _repositoryMock.Verify(r => r.UpdateAsync(It.Is<Character>(c => c.Id == character.Id)), Times.Once);
    }
  }
}
