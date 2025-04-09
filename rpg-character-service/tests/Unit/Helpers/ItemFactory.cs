using RPGCharacterService.Entities.Items;

namespace RPGCharacterService.Tests.Unit.Helpers {
  public static class ItemFactory {
    public static Item CreateItem(int? id = null, string? name = null, EquipmentStats? equipmentStats = null) {
      return new Item {
        Id = id ?? 1,
        Name = name ?? "Test Item",
        EquipmentStats = equipmentStats
      };
    }

    public static Item CreateWeapon(int? id = null,
                                    string? name = null,
                                    WeaponProperty weaponProperties = WeaponProperty.None,
                                    WeaponRangeType rangeType = WeaponRangeType.Melee) {
      return CreateItem(id,
                        name: name ?? "Test Weapon",
                        equipmentStats: new EquipmentStats {
                          EquipmentType = EquipmentType.Weapon,
                          WeaponStats = new WeaponStats {
                            WeaponProperties = weaponProperties,
                            RangeType = rangeType
                          }
                        });
    }

    public static Item CreateArmor(int? id = null,
                                   string? name = null,
                                   int baseArmorClass = 10,
                                   ArmorType armorType = ArmorType.Light) {
      return CreateItem(id,
                        name: name ?? "Test Armor",
                        equipmentStats: new EquipmentStats {
                          EquipmentType = EquipmentType.Armor,
                          ArmorStats = new ArmorStats {
                            BaseArmorClass = baseArmorClass,
                            ArmorType = armorType
                          }
                        });
    }

    public static Item CreateShield(int? id = null, string? name = null, int baseArmorClass = 2) {
      return CreateItem(name: name ?? "Test Shield",
                        equipmentStats: new EquipmentStats {
                          EquipmentType = EquipmentType.Shield,
                          ArmorStats = new ArmorStats {
                            BaseArmorClass = baseArmorClass,
                            ArmorType = ArmorType.None
                          }
                        });
    }

    public static Item CreateTwoHandedWeapon(int? id = null,
                                             string? name = null,
                                             WeaponRangeType rangeType = WeaponRangeType.Melee) {
      return CreateWeapon(id,
                          name: name ?? "Test Two-Handed Weapon",
                          weaponProperties: WeaponProperty.TwoHanded,
                          rangeType: rangeType);
    }

    public static Item CreateFinesseWeapon(int? id = null,
                                           string? name = null,
                                           WeaponRangeType rangeType = WeaponRangeType.Melee) {
      return CreateWeapon(id,
                          name: name ?? "Test Finesse Weapon",
                          weaponProperties: WeaponProperty.Finesse,
                          rangeType: rangeType);
    }

    public static Item CreateRangedWeapon(int? id = null,
                                          string? name = null,
                                          WeaponProperty weaponProperties = WeaponProperty.None) {
      return CreateWeapon(id,
                          name: name ?? "Test Ranged Weapon",
                          weaponProperties: weaponProperties,
                          rangeType: WeaponRangeType.Ranged);
    }
  }
}
