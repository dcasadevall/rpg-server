using RPGCharacterService.Entities.Characters;
using RPGCharacterService.Entities.Items;

namespace RPGCharacterService.UnitTests.Models {
  public class CharacterTests {
    [Theory]
    [InlineData(10, 1, 10)] // Base 10 + (0 modifier * 1 level)
    [InlineData(12, 1, 11)] // Base 10 + (1 modifier * 1 level)
    [InlineData(14, 1, 12)] // Base 10 + (2 modifier * 1 level)
    [InlineData(10, 5, 10)] // Base 10 + (0 modifier * 5 levels)
    [InlineData(12, 5, 15)] // Base 10 + (1 modifier * 5 levels)
    [InlineData(14, 5, 20)] // Base 10 + (2 modifier * 5 levels)
    public void CalculateMaxHitPoints_WithDifferentConstitutionAndLevels_ShouldReturnCorrectValue(
      int constitution,
      int level,
      int expectedHitPoints) {
      // Arrange
      var character = new Character {
        Level = level,
        AbilityScores = new Dictionary<AbilityScore, int> {
          {AbilityScore.Constitution, constitution}
        }
      };

      // Act
      var result = character.CalculateMaxHitPoints();

      // Assert
      Assert.Equal(expectedHitPoints, result);
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(4, 2)]
    [InlineData(5, 3)]
    [InlineData(8, 3)]
    [InlineData(9, 4)]
    [InlineData(12, 4)]
    [InlineData(13, 5)]
    [InlineData(16, 5)]
    [InlineData(17, 6)]
    [InlineData(20, 6)]
    public void CalculateProficiencyBonus_WithDifferentLevels_ShouldReturnCorrectValue(int level, int expectedBonus) {
      // Arrange
      var character = new Character {Level = level};

      // Act
      var result = character.CalculateProficiencyBonus();

      // Assert
      Assert.Equal(expectedBonus, result);
    }

    [Theory]
    [InlineData(1, -5)]
    [InlineData(3, -4)]
    [InlineData(5, -3)]
    [InlineData(7, -2)]
    [InlineData(9, -1)]
    [InlineData(10, 0)]
    [InlineData(11, 0)]
    [InlineData(12, 1)]
    [InlineData(14, 2)]
    [InlineData(16, 3)]
    [InlineData(18, 4)]
    [InlineData(20, 5)]
    public void CalculateAbilityModifier_WithDifferentScores_ShouldReturnCorrectValue(int score, int expectedModifier) {
      // Arrange
      var character = new Character {
        AbilityScores = new Dictionary<AbilityScore, int> {
          {AbilityScore.Strength, score}
        }
      };

      // Act
      var result = character.CalculateAbilityModifier(AbilityScore.Strength);

      // Assert
      Assert.Equal(expectedModifier, result);
    }

    [Fact]
    public void CalculateAllAbilityModifiers_WithMultipleScores_ShouldReturnCorrectModifiers() {
      // Arrange
      var character = new Character {
        AbilityScores = new Dictionary<AbilityScore, int> {
          {AbilityScore.Strength, 10}, // Modifier: 0
          {AbilityScore.Dexterity, 12}, // Modifier: 1
          {AbilityScore.Constitution, 14}, // Modifier: 2
          {AbilityScore.Intelligence, 8}, // Modifier: -1
          {AbilityScore.Wisdom, 16}, // Modifier: 3
          {AbilityScore.Charisma, 18} // Modifier: 4
        }
      };

      // Act
      var result = character.CalculateAllAbilityModifiers();

      // Assert
      Assert.Equal(0, result[AbilityScore.Strength]);
      Assert.Equal(1, result[AbilityScore.Dexterity]);
      Assert.Equal(2, result[AbilityScore.Constitution]);
      Assert.Equal(-1, result[AbilityScore.Intelligence]);
      Assert.Equal(3, result[AbilityScore.Wisdom]);
      Assert.Equal(4, result[AbilityScore.Charisma]);
    }

    [Fact]
    public void CalculateArmorClass_ShouldForwardToEquipment() {
      // Arrange
      var character = new Character {
        AbilityScores = new Dictionary<AbilityScore, int> {
          {AbilityScore.Dexterity, 14} // Modifier: 2
        }
      };
      var armor = new Item {
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Armor,
          ArmorStats = new ArmorStats {
            ArmorType = ArmorType.Light,
            BaseArmorClass = 11
          }
        }
      };
      character.Equipment.EquipArmor(armor);

      // Act
      var result = character.CalculateArmorClass();

      // Assert
      Assert.Equal(13, result); // 11 (base) + 2 (dex)
    }

    [Fact]
    public void CalculateWeaponDamageModifier_ShouldForwardToEquipment() {
      // Arrange
      var character = new Character {
        AbilityScores = new Dictionary<AbilityScore, int> {
          {AbilityScore.Strength, 16}, // Modifier: 3
          {AbilityScore.Dexterity, 14} // Modifier: 2
        }
      };
      var weapon = new Item {
        EquipmentStats = new EquipmentStats {
          EquipmentType = EquipmentType.Weapon,
          WeaponStats = new WeaponStats {
            WeaponProperties = WeaponProperty.Finesse
          }
        }
      };
      character.Equipment.EquipWeapon(weapon);

      // Act
      var result = character.CalculateWeaponDamageModifier();

      // Assert
      Assert.Equal(AbilityScore.Strength, result); // STR is higher than DEX
    }

    [Fact]
    public void CalculateWeaponModifier_WithRangedWeapon_ShouldReturnDex() {
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
  }
}
