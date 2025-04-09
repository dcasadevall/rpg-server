using RPGCharacterService.Exceptions.Character;
using RPGCharacterService.Exceptions.Items;
using RPGCharacterService.Entities.Characters;
using RPGCharacterService.Entities.Items;
using RPGCharacterService.Persistence;
using RPGCharacterService.Services;

namespace RPGCharacterService.UnitTests.Services {
  public class EquipmentServiceTests {
    private readonly Mock<ICharacterRepository> characterRepositoryMock;
    private readonly Mock<IItemRepository> itemRepositoryMock;
    private readonly EquipmentService equipmentService;

    public EquipmentServiceTests() {
      characterRepositoryMock = new Mock<ICharacterRepository>();
      itemRepositoryMock = new Mock<IItemRepository>();
      equipmentService = new EquipmentService(characterRepositoryMock.Object, itemRepositoryMock.Object);
    }

    [Fact]
    public async Task EquipWeapon_WithValidCharacterAndWeapon_ShouldUpdateCharacter() {
      // Arrange
      var character = new Character();
      var characterId = character.Id;
      var weaponId = 1;
      var weapon = new Item {
        Id = weaponId,
        Name = "Test Weapon",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Weapon,
        }
      };

      characterRepositoryMock.Setup(x => x.GetByIdAsync(characterId))
        .ReturnsAsync(character);
      itemRepositoryMock.Setup(x => x.GetByIdAsync(weaponId))
        .ReturnsAsync(weapon);

      // Act
      await equipmentService.EquipWeaponAsync(characterId, weaponId);

      // Assert
      characterRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Character>(c =>
        c.Id == characterId &&
        c.Equipment.MainHand != null &&
        c.Equipment.MainHand.Id == weaponId)), Times.Once);
    }

    [Fact]
    public async Task EquipWeapon_WithNonExistentCharacter_ShouldThrowCharacterNotFoundException() {
      // Arrange
      var characterId = Guid.NewGuid();
      var weaponId = 1;
      var weapon = new Item {
        Id = weaponId,
        Name = "Test Weapon",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Weapon,
        }
      };

      characterRepositoryMock.Setup(x => x.GetByIdAsync(characterId))
        .ReturnsAsync((Character?)null);
      itemRepositoryMock.Setup(x => x.GetByIdAsync(weaponId))
        .ReturnsAsync(weapon);

      // Act & Assert
      await Assert.ThrowsAsync<CharacterNotFoundException>(() =>
        equipmentService.EquipWeaponAsync(characterId, weaponId));
    }

    [Fact]
    public async Task EquipWeapon_WithNonExistentWeapon_ShouldThrowItemNotFoundException() {
      // Arrange
      var character = new Character();
      var characterId = character.Id;
      var weaponId = 1;

      characterRepositoryMock.Setup(x => x.GetByIdAsync(characterId))
        .ReturnsAsync(character);
      itemRepositoryMock.Setup(x => x.GetByIdAsync(weaponId))
        .ReturnsAsync((Item?)null);

      // Act & Assert
      await Assert.ThrowsAsync<ItemNotFoundException>(() =>
        equipmentService.EquipWeaponAsync(characterId, weaponId));
    }

    [Fact]
    public async Task EquipArmor_WithValidCharacterAndArmor_ShouldUpdateCharacter() {
      // Arrange
      var character = new Character();
      var characterId = character.Id;
      var armorId = 1;
      var armor = new Item {
        Id = armorId,
        Name = "Test Armor",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Armor,
        }
      };

      characterRepositoryMock.Setup(x => x.GetByIdAsync(characterId))
        .ReturnsAsync(character);
      itemRepositoryMock.Setup(x => x.GetByIdAsync(armorId))
        .ReturnsAsync(armor);

      // Act
      await equipmentService.EquipArmorAsync(characterId, armorId);

      // Assert
      characterRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Character>(c =>
        c.Id == characterId &&
        c.Equipment.Armor != null &&
        c.Equipment.Armor.Id == armorId)), Times.Once);
    }

    [Fact]
    public async Task EquipShield_WithValidCharacterAndShield_ShouldUpdateCharacter() {
      // Arrange
      var character = new Character();
      var characterId = character.Id;
      var shieldId = 1;
      var shield = new Item {
        Id = shieldId,
        Name = "Test Shield",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Shield,
        }
      };

      characterRepositoryMock.Setup(x => x.GetByIdAsync(characterId))
        .ReturnsAsync(character);
      itemRepositoryMock.Setup(x => x.GetByIdAsync(shieldId))
        .ReturnsAsync(shield);

      // Act
      await equipmentService.EquipShieldAsync(characterId, shieldId);

      // Assert
      characterRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Character>(c =>
        c.Id == characterId &&
        c.Equipment.OffHand != null &&
        c.Equipment.OffHand.Id == shieldId &&
        c.Equipment.OffHand.IsShield())), Times.Once);
    }

    [Theory]
    [InlineData(false)] // Main hand
    [InlineData(true)]  // Off-hand
    public async Task EquipWeapon_WithValidCharacterAndWeapon_ShouldUpdateCorrectHand(bool isOffHand) {
      // Arrange
      var character = new Character();
      var characterId = character.Id;
      var weaponId = 1;
      var weapon = new Item {
        Id = weaponId,
        Name = "Test Weapon",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Weapon,
        }
      };

      characterRepositoryMock.Setup(x => x.GetByIdAsync(characterId))
        .ReturnsAsync(character);
      itemRepositoryMock.Setup(x => x.GetByIdAsync(weaponId))
        .ReturnsAsync(weapon);

      // Act
      await equipmentService.EquipWeaponAsync(characterId, weaponId, isOffHand);

      // Assert
      characterRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Character>(c =>
        c.Id == characterId &&
        (isOffHand ?
          (c.Equipment.OffHand != null && c.Equipment.OffHand.Id == weaponId) :
          (c.Equipment.MainHand != null && c.Equipment.MainHand.Id == weaponId)))), Times.Once);
    }
  }
}
