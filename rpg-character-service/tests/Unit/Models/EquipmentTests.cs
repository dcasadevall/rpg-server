using RPGCharacterService.Exceptions.Equipment;
using RPGCharacterService.Models;
using RPGCharacterService.Models.Items;
using RPGCharacterService.Models.Characters;

namespace RPGCharacterService.UnitTests.Models {
  public class EquipmentTests {
    [Theory]
    [InlineData(false)] // Main hand
    [InlineData(true)] // Off-hand
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
    [InlineData(true)] // Offhand
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
    [InlineData(true)] // Offhand
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
          WeaponStats = new WeaponStats {
            WeaponProperties = WeaponProperty.TwoHanded
          }
        }
      };

      // Arrange: Set Up previous state
      equipment.EquipWeapon(oldMainHand);
      equipment.EquipShield(oldOffHand);
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
          WeaponStats = new WeaponStats {
            WeaponProperties = WeaponProperty.TwoHanded
          }
        }
      };

      // Act & Assert
      Assert.Throws<IllegalEquipmentStateException>(() => equipment.EquipWeapon(twoHandedWeapon, true));
    }

    [Fact]
    public void EquipWeapon_WithTwoHandedWeaponEquipped_ShouldClearMainHand() {
      // Arrange
      var equipment = new Equipment();
      var twoHandedWeapon = new Item {
        Id = 1,
        Name = "TwoHanded",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Weapon,
          WeaponStats = new WeaponStats {
            WeaponProperties = WeaponProperty.TwoHanded
          }
        }
      };

      var newWeapon = new Item {
        Id = 2,
        Name = "Test Item 2",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Weapon,
        }
      };
      // Arrange: Set Up previous state
      equipment.EquipWeapon(twoHandedWeapon);
      Assert.Equal(twoHandedWeapon.Id, equipment.MainHand?.Id);

      // Act
      equipment.EquipWeapon(newWeapon);

      // Assert
      Assert.Equal(newWeapon.Id, equipment.MainHand?.Id);
      Assert.Null(equipment.OffHand);
    }

    [Fact]
    public void EquipShield_WithTwoHandedWeaponEquipped_ShouldClearMainHand() {
      // Arrange
      var equipment = new Equipment();
      var twoHandedWeapon = new Item {
        Id = 1,
        Name = "TwoHanded",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Weapon,
          WeaponStats = new WeaponStats {
            WeaponProperties = WeaponProperty.TwoHanded
          }
        }
      };

      var shield = new Item {
        Id = 2,
        Name = "Shield",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Shield,
        }
      };
      // Arrange: Set Up previous state
      equipment.EquipWeapon(twoHandedWeapon);
      Assert.Equal(twoHandedWeapon.Id, equipment.MainHand?.Id);

      // Act
      equipment.EquipShield(shield);

      // Assert
      Assert.Null(equipment.MainHand);
      Assert.Equal(shield.Id, equipment.OffHand?.Id);
    }

    [Fact]
    public void EquipShield_WithOffHandWeapon_ShouldReplaceOffHand() {
      // Arrange
      var equipment = new Equipment();
      var oldOffHand = new Item {
        Id = 1,
        Name = "Test Item",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Weapon,
        }
      };
      var oldMainHand = new Item {
        Id = 2,
        Name = "Test Item 2",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Weapon,
        }
      };
      var newShield = new Item {
        Id = 2,
        Name = "Test Item 2",
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Shield,
        }
      };

      // Arrange: Set Up previous state
      equipment.EquipWeapon(oldMainHand);
      equipment.EquipWeapon(oldOffHand, true);
      Assert.Equal(oldMainHand.Id, equipment.MainHand?.Id);
      Assert.Equal(oldOffHand.Id, equipment.OffHand?.Id);

      // Act
      equipment.EquipShield(newShield);

      // Assert
      Assert.Equal(newShield.Id, equipment.OffHand?.Id);
    }

    [Theory]
    [InlineData(ArmorType.None, 0, 0, 10)] // No armor, no shield, base AC 10
    [InlineData(ArmorType.None, 2, 0, 12)] // No armor, no shield, +2 dex
    [InlineData(ArmorType.None, 0, 2, 12)] // No armor, +2 shield
    [InlineData(ArmorType.Light, 0, 0, 11)] // Light armor, no shield, base AC 11
    [InlineData(ArmorType.Light, 2, 0, 13)] // Light armor, no shield, +2 dex
    [InlineData(ArmorType.Medium, 0, 0, 13)] // Medium armor, no shield, base AC 13
    [InlineData(ArmorType.Medium, 2, 0, 15)] // Medium armor, no shield, +2 dex (capped)
    [InlineData(ArmorType.Medium, 4, 0, 15)] // Medium armor, no shield, +4 dex (still capped)
    [InlineData(ArmorType.Heavy, 0, 0, 18)] // Heavy armor, no shield, base AC 18
    [InlineData(ArmorType.Heavy, 2, 0, 18)] // Heavy armor, no shield, +2 dex (ignored)
    public void CalculateArmorClass_WithDifferentArmorTypes_ShouldReturnCorrectValue(
      ArmorType armorType,
      int dexterityModifier,
      int shieldBonus,
      int expectedArmorClass) {
      // Arrange
      var equipment = new Equipment();
      if (armorType != ArmorType.None) {
        equipment.EquipArmor(new Item {
          EquipmentStats = new EquipmentStats {
            EquipmentType = EquipmentType.Armor,
            ArmorStats = new ArmorStats {
              ArmorType = armorType,
              BaseArmorClass = armorType switch {
                ArmorType.Light => 11,
                ArmorType.Medium => 13,
                ArmorType.Heavy => 18,
                _ => 0
              }
            }
          }
        });
      }
      if (shieldBonus > 0) {
        equipment.EquipShield(new Item {
          EquipmentStats = new EquipmentStats {
            EquipmentType = EquipmentType.Shield,
          }
        });
      }

      // Act
      var result = equipment.CalculateArmorClass(dexterityModifier);

      // Assert
      Assert.Equal(expectedArmorClass, result);
    }

    [Theory]
    [InlineData(WeaponProperty.Finesse, 3, 2, AbilityScore.Strength)] // Finesse weapon, higher STR
    [InlineData(WeaponProperty.Finesse, 2, 3, AbilityScore.Dexterity)] // Finesse weapon, higher DEX
    [InlineData(WeaponProperty.None, 3, 2, AbilityScore.Strength)] // Non-finesse weapon, always STR
    [InlineData(WeaponProperty.None, 2, 3, AbilityScore.Strength)] // Non-finesse weapon, always STR
    public void CalculateWeaponDamageModifier_WithDifferentWeapons_ShouldReturnCorrectAbility(
      WeaponProperty weaponProperty,
      int strengthModifier,
      int dexterityModifier,
      AbilityScore expected) {
      // Arrange
      var equipment = new Equipment();
      var weapon = new Item {
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Weapon,
          WeaponStats = new WeaponStats {
            WeaponProperties = weaponProperty,
            RangeType = WeaponRangeType.Melee
          }
        }
      };
      equipment.EquipWeapon(weapon);

      var abilityModifiers = new Dictionary<AbilityScore, int> {
        {AbilityScore.Strength, strengthModifier},
        {AbilityScore.Dexterity, dexterityModifier}
      };

      // Act
      var result = equipment.CalculateWeaponDamageModifier(abilityModifiers);

      // Assert
      Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculateWeaponDamageModifier_WithRangedWeapon_ShouldReturnDexterity() {
      // Arrange
      var rangedWeapon = new Item {
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Weapon,
          WeaponStats = new WeaponStats {
            RangeType = WeaponRangeType.Ranged,
          }
        }
      };
      var character = new Character {
        AbilityScores = new Dictionary<AbilityScore, int> {
          {AbilityScore.Strength, 10}, // Modifier: 0
          {AbilityScore.Dexterity, 16} // Modifier: 3
        },
      };
      character.Equipment.EquipWeapon(rangedWeapon);
      Assert.NotNull(character.Equipment.MainHand);
      Assert.Equal(rangedWeapon.Id, character.Equipment.MainHand.Id);

      // Act
      var result = character.CalculateWeaponDamageModifier();

      // Assert
      Assert.Equal(AbilityScore.Dexterity, result);
    }

    [Fact]
    public void CalculateWeaponDamageModifier_WithNoWeapon_ShouldReturnStrength() {
      // Arrange
      var equipment = new Equipment();
      var abilityModifiers = new Dictionary<AbilityScore, int> {
        {AbilityScore.Strength, 3},
        {AbilityScore.Dexterity, 2}
      };

      // Act
      var result = equipment.CalculateWeaponDamageModifier(abilityModifiers);

      // Assert
      Assert.Equal(AbilityScore.Strength, result);
    }


    [Theory]
    [InlineData(10, 10)] // No armor, no dex mod
    [InlineData(12, 11)] // No armor, +1 dex mod
    [InlineData(14, 12)] // No armor, +2 dex
    [InlineData(8, 9)] // No armor, -1 dex
    public void CalculateArmorClass_WithNoArmor_ShouldReturnBasePlusDexterity(
      int dexterity,
      int expectedArmorClass) {
      // Arrange
      var character = new Character {
        AbilityScores = new Dictionary<AbilityScore, int> {
          {AbilityScore.Dexterity, dexterity}
        }
      };

      // Act
      var result = character.CalculateArmorClass();

      // Assert
      Assert.Equal(expectedArmorClass, result);
    }

    [Theory]
    [InlineData(16, 10, AbilityScore.Strength)] // 15 STR 10 DEX
    [InlineData(4, 11, AbilityScore.Dexterity)] // 10 STR 11 STR
    public void CalculateWeaponDamageModifier_WithFinesseWeapon_ShouldReturnHighestDexOrStr(
      int str,
      int dex,
      AbilityScore expected) {
      // Arrange
      var character = new Character {
        AbilityScores = new Dictionary<AbilityScore, int> {
          {AbilityScore.Strength, str},
          {AbilityScore.Dexterity, dex}
        }
      };
      var finesseWeapon = new Item {
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Weapon,
          WeaponStats = new WeaponStats {
            WeaponProperties = WeaponProperty.Finesse
          }
        }
      };
      character.Equipment.EquipWeapon(finesseWeapon);
      Assert.NotNull(character.Equipment.MainHand);
      Assert.Equal(finesseWeapon.Id, character.Equipment.MainHand.Id);

      // Act
      var result = character.CalculateWeaponDamageModifier();

      // Assert
      Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculateWeaponDamageModifier_WithNonFinesseWeapon_ShouldReturnStrength() {
      // Arrange
      var character = new Character {
        AbilityScores = new Dictionary<AbilityScore, int> {
          {AbilityScore.Strength, 16}, // Modifier: 3
          {AbilityScore.Dexterity, 10} // Modifier: 0
        }
      };
      var nonFinesseWeapon = new Item {
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Weapon,
          WeaponStats = new WeaponStats {
            WeaponProperties = WeaponProperty.None
          }
        }
      };
      character.Equipment.EquipWeapon(nonFinesseWeapon);
      Assert.NotNull(character.Equipment.MainHand);
      Assert.Equal(nonFinesseWeapon.Id, character.Equipment.MainHand.Id);

      // Act
      var result = character.CalculateWeaponDamageModifier();

      // Assert
      Assert.Equal(AbilityScore.Strength, result);
    }
  }
}
