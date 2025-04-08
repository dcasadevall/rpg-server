using RPGCharacterService.Models.Items;

namespace RPGCharacterService.UnitTests.Models {
  public class EquipmentExtensionsTests {
    [Fact]
    public void IsWeapon_WithWeaponItem_ShouldReturnTrue() {
      // Arrange
      var item = new Item {
        Id = 1,
        Name = "Test Weapon",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Weapon,
        }
      };

      // Act
      var result = item.IsWeapon();

      // Assert
      Assert.True(result);
    }

    [Fact]
    public void IsWeapon_WithNonWeaponItem_ShouldReturnFalse() {
      // Arrange
      var item = new Item {
        Id = 1,
        Name = "Test Armor",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Armor,
        }
      };

      // Act
      var result = item.IsWeapon();

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void IsArmor_WithArmorItem_ShouldReturnTrue() {
      // Arrange
      var item = new Item {
        Id = 1,
        Name = "Test Armor",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Armor,
        }
      };

      // Act
      var result = item.IsArmor();

      // Assert
      Assert.True(result);
    }

    [Fact]
    public void IsArmor_WithNonArmorItem_ShouldReturnFalse() {
      // Arrange
      var item = new Item {
        Id = 1,
        Name = "Test Weapon",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Weapon,
        }
      };

      // Act
      var result = item.IsArmor();

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void IsShield_WithShieldItem_ShouldReturnTrue() {
      // Arrange
      var item = new Item {
        Id = 1,
        Name = "Test Shield",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Shield,
        }
      };

      // Act
      var result = item.IsShield();

      // Assert
      Assert.True(result);
    }

    [Fact]
    public void IsShield_WithNonShieldItem_ShouldReturnFalse() {
      // Arrange
      var item = new Item {
        Id = 1,
        Name = "Test Weapon",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Weapon,
        }
      };

      // Act
      var result = item.IsShield();

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void IsWeapon_WithNullEquipmentStats_ShouldReturnFalse() {
      // Arrange
      var item = new Item {
        Id = 1,
        Name = "Test Item",
        EquipmentStats = null
      };

      // Act
      var result = item.IsWeapon();

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void IsArmor_WithNullEquipmentStats_ShouldReturnFalse() {
      // Arrange
      var item = new Item {
        Id = 1,
        Name = "Test Item",
        EquipmentStats = null
      };

      // Act
      var result = item.IsArmor();

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void IsShield_WithNullEquipmentStats_ShouldReturnFalse() {
      // Arrange
      var item = new Item {
        Id = 1,
        Name = "Test Item",
        EquipmentStats = null
      };

      // Act
      var result = item.IsShield();

      // Assert
      Assert.False(result);
    }
  }
}
