using RPGCharacterService.Exceptions.Character;
using RPGCharacterService.Exceptions.Items;
using RPGCharacterService.Models.Characters;
using RPGCharacterService.Models.Items;
using RPGCharacterService.Persistence.Characters;
using RPGCharacterService.Persistence.Items;
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
      var characterId = Guid.NewGuid();
      var weaponId = Guid.NewGuid();
      var character = new Character { Id = characterId };
      var weapon = new Item { Id = weaponId, Type = ItemType.Weapon };

      characterRepositoryMock.Setup(x => x.GetByIdAsync(characterId))
        .ReturnsAsync(character);
      itemRepositoryMock.Setup(x => x.GetByIdAsync(weaponId))
        .ReturnsAsync(weapon);

      // Act
      await equipmentService.EquipWeapon(characterId, weaponId);

      // Assert
      characterRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Character>(c =>
        c.Id == characterId &&
        c.Equipment.Weapon == weapon)), Times.Once);
    }

    [Fact]
    public async Task EquipWeapon_WithNonExistentCharacter_ShouldThrowCharacterNotFoundException() {
      // Arrange
      var characterId = Guid.NewGuid();
      var weaponId = Guid.NewGuid();

      characterRepositoryMock.Setup(x => x.GetByIdAsync(characterId))
        .ReturnsAsync((Character)null);

      // Act & Assert
      await Assert.ThrowsAsync<CharacterNotFoundException>(() =>
        equipmentService.EquipWeapon(characterId, weaponId));
    }

    [Fact]
    public async Task EquipWeapon_WithNonExistentWeapon_ShouldThrowItemNotFoundException() {
      // Arrange
      var characterId = Guid.NewGuid();
      var weaponId = Guid.NewGuid();
      var character = new Character { Id = characterId };

      characterRepositoryMock.Setup(x => x.GetByIdAsync(characterId))
        .ReturnsAsync(character);
      itemRepositoryMock.Setup(x => x.GetByIdAsync(weaponId))
        .ReturnsAsync((Item)null);

      // Act & Assert
      await Assert.ThrowsAsync<ItemNotFoundException>(() =>
        equipmentService.EquipWeapon(characterId, weaponId));
    }

    [Fact]
    public async Task EquipArmor_WithValidCharacterAndArmor_ShouldUpdateCharacter() {
      // Arrange
      var characterId = Guid.NewGuid();
      var armorId = Guid.NewGuid();
      var character = new Character { Id = characterId };
      var armor = new Item { Id = armorId, Type = ItemType.Armor };

      characterRepositoryMock.Setup(x => x.GetByIdAsync(characterId))
        .ReturnsAsync(character);
      itemRepositoryMock.Setup(x => x.GetByIdAsync(armorId))
        .ReturnsAsync(armor);

      // Act
      await equipmentService.EquipArmor(characterId, armorId);

      // Assert
      characterRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Character>(c =>
        c.Id == characterId &&
        c.Equipment.Armor == armor)), Times.Once);
    }

    [Fact]
    public async Task EquipShield_WithValidCharacterAndShield_ShouldUpdateCharacter() {
      // Arrange
      var characterId = Guid.NewGuid();
      var shieldId = Guid.NewGuid();
      var character = new Character { Id = characterId };
      var shield = new Item { Id = shieldId, Type = ItemType.Shield };

      characterRepositoryMock.Setup(x => x.GetByIdAsync(characterId))
        .ReturnsAsync(character);
      itemRepositoryMock.Setup(x => x.GetByIdAsync(shieldId))
        .ReturnsAsync(shield);

      // Act
      await equipmentService.EquipShield(characterId, shieldId);

      // Assert
      characterRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Character>(c =>
        c.Id == characterId &&
        c.Equipment.Shield == shield)), Times.Once);
    }
  }
}
