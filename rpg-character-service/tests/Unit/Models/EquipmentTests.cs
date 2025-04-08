using RPGCharacterService.Exceptions.Equipment;
using RPGCharacterService.Models;
using RPGCharacterService.Models.Items;

namespace RPGCharacterService.UnitTests.Models {
  public class EquipmentTests {
    [Theory]
    [InlineData(false)] // Main hand
    [InlineData(true)]  // Off-hand
    public void EquipWeapon_WithValidWeapon_ShouldEquipWeapon(bool isOffHand) {
      // Arrange
      var equipment = new Equipment();
      var weapon = new Item {
        Id = 1,
        Name = "Test Item",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Weapon,
        }
      };

      // Act
      equipment.EquipWeapon(weapon, isOffHand);

      // Assert
      if (isOffHand) {
        Assert.Equal(weapon.Id, equipment.OffHand?.Id);
        Assert.Null(equipment.MainHand);
      } else {
        Assert.Equal(weapon.Id, equipment.MainHand?.Id);
        Assert.Null(equipment.OffHand);
      }
    }

    [Theory]
    [InlineData(false)] // Main hand
    [InlineData(true)]  // Off hand
    public void EquipWeapon_WithNonWeaponItem_ShouldThrowEquipmentTypeMismatchException(bool isOffHand) {
      // Arrange
      var equipment = new Equipment();
      var armor = new Item {
        Id = 1,
        Name = "Test Item",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Armor,
        }
      };

      // Act & Assert
      Assert.Throws<EquipmentTypeMismatchException>(() => equipment.EquipWeapon(armor, isOffHand));
    }

    [Fact]
    public void EquipArmor_WithValidArmor_ShouldEquipArmor() {
      // Arrange
      var equipment = new Equipment();
      var armor = new Item {
        Id = 1,
        Name = "Test Item",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Armor,
        }
      };

      // Act
      equipment.EquipArmor(armor);

      // Assert
      Assert.Equal(armor.Id, equipment.Armor?.Id);
    }

    [Fact]
    public void EquipArmor_WithNonArmorItem_ShouldThrowEquipmentTypeMismatchException() {
      // Arrange
      var equipment = new Equipment();
      var weapon = new Item {
        Id = 1,
        Name = "Test Item",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Weapon,
        }
      };

      // Act & Assert
      Assert.Throws<EquipmentTypeMismatchException>(() => equipment.EquipArmor(weapon));
    }

    [Fact]
    public void EquipShield_WithValidShield_ShouldEquipShield() {
      // Arrange
      var equipment = new Equipment();
      var shield = new Item {
        Id = 1,
        Name = "Test Item",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Shield,
        }
      };

      // Act
      equipment.EquipShield(shield);

      // Assert
      Assert.Equal(shield.Id, equipment.OffHand?.Id);
      Assert.True(equipment.OffHand?.IsShield());
    }

    [Fact]
    public void EquipShield_WithNonShieldItem_ShouldThrowEquipmentTypeMismatchException() {
      // Arrange
      var equipment = new Equipment();
      var weapon = new Item {
        Id = 1,
        Name = "Test Item",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Weapon,
        }
      };

      // Act & Assert
      Assert.Throws<EquipmentTypeMismatchException>(() => equipment.EquipShield(weapon));
    }

    [Theory]
    [InlineData(false)] // Main hand
    [InlineData(true)]  // Off hand
    public void EquipWeapon_WithExistingWeapon_ShouldReplaceWeapon(bool isOffHand) {
      // Arrange
      var equipment = new Equipment();
      var oldWeapon = new Item {
        Id = 1,
        Name = "Test Item",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Weapon,
        }
      };

      var newWeapon = new Item {
        Id = 2,
        Name = "Test Item 2",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Weapon,
        }
      };

      // Act
      equipment.EquipWeapon(oldWeapon, isOffHand);
      equipment.EquipWeapon(newWeapon, isOffHand);

      // Assert
      if (isOffHand) {
        Assert.Equal(newWeapon.Id, equipment.OffHand?.Id);
        Assert.Null(equipment.MainHand);
      } else {
        Assert.Equal(newWeapon.Id, equipment.MainHand?.Id);
        Assert.Null(equipment.OffHand);
      }
    }

    [Fact]
    public void EquipWeapon_WithTwoHandedWeapon_ShouldClearBothHands() {
      // Arrange
      var equipment = new Equipment();
      var oldMainHand = new Item {
        Id = 1,
        Name = "Test Item",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Weapon,
        }
      };
      var oldOffHand = new Item {
        Id = 2,
        Name = "Test Item 2",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Shield,
        }
      };
      var twoHandedWeapon = new Item {
        Id = 3,
        Name = "TwoHanded",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Weapon,
        }
      };

      // Act & Assert (Equip old weapons)
      equipment.EquipWeapon(oldMainHand);
      equipment.EquipWeapon(oldOffHand, true);
      Assert.Equal(oldMainHand.Id, equipment.MainHand?.Id);
      Assert.Equal(oldOffHand.Id, equipment.OffHand?.Id);

      // Act
      equipment.EquipWeapon(twoHandedWeapon);

      // Assert
      Assert.Equal(twoHandedWeapon.Id, equipment.MainHand?.Id);
      Assert.Null(equipment.OffHand);
    }

    [Fact]
    public void EquipWeapon_WithTwoHandedWeaponInOffHand_ShouldThrowIllegalEquipmentStateException() {
      // Arrange
      var equipment = new Equipment();
      var twoHandedWeapon = new Item {
        Id = 1,
        Name = "TwoHanded",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Weapon,
        }
      };

      // Act & Assert
      Assert.Throws<IllegalEquipmentStateException>(() => equipment.EquipWeapon(twoHandedWeapon, true));
    }
  }
}
