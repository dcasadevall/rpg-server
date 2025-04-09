using RPGCharacterService.Entities.Items;

namespace RPGCharacterService.UnitTests.Models.Items {
  public class WeaponExtensionsTests {
    [Fact]
    public void IsTwoHandedWeapon_WithTwoHandedWeapon_ShouldReturnTrue() {
      // Arrange
      var item = new Item {
        Id = 1,
        Name = "Test Weapon",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Weapon,
          WeaponStats = new WeaponStats {
            WeaponProperties = WeaponProperty.TwoHanded
          }
        }
      };

      // Act
      var result = item.IsTwoHandedWeapon();

      // Assert
      Assert.True(result);
    }

    [Fact]
    public void IsTwoHandedWeapon_WithOneHandedWeapon_ShouldReturnFalse() {
      // Arrange
      var item = new Item {
        Id = 1,
        Name = "Test Weapon",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Weapon,
          WeaponStats = new WeaponStats {
            WeaponProperties = WeaponProperty.None
          }
        }
      };

      // Act
      var result = item.IsTwoHandedWeapon();

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void IsTwoHandedWeapon_WithNullWeaponStats_ShouldReturnFalse() {
      // Arrange
      var item = new Item {
        Id = 1,
        Name = "Test Weapon",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Weapon,
          WeaponStats = null
        }
      };

      // Act
      var result = item.IsTwoHandedWeapon();

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void IsTwoHandedWeapon_WithNullEquipmentStats_ShouldReturnFalse() {
      // Arrange
      var item = new Item {
        Id = 1,
        Name = "Test Weapon",
        EquipmentStats = null
      };

      // Act
      var result = item.IsTwoHandedWeapon();

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void IsTwoHandedWeapon_WithNonWeaponItem_ShouldReturnFalse() {
      // Arrange
      var item = new Item {
        Id = 1,
        Name = "Test Armor",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Armor,
          WeaponStats = new WeaponStats {
            WeaponProperties = WeaponProperty.TwoHanded
          }
        }
      };

      // Act
      var result = item.IsTwoHandedWeapon();

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void IsTwoHandedWeapon_WithMultipleProperties_ShouldReturnTrue() {
      // Arrange
      var item = new Item {
        Id = 1,
        Name = "Test Weapon",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Weapon,
          WeaponStats = new WeaponStats {
            WeaponProperties = WeaponProperty.TwoHanded | WeaponProperty.Finesse
          }
        }
      };

      // Act
      var result = item.IsTwoHandedWeapon();

      // Assert
      Assert.True(result);
    }
  }
}
