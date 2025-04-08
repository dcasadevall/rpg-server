using RPGCharacterService.Exceptions.Items;
using RPGCharacterService.Models.Characters;
using RPGCharacterService.Models.Items;
using RPGCharacterService.Persistence.Characters;
using RPGCharacterService.Persistence.Items;

namespace RPGCharacterService.Services {
  public interface IEquipmentService {
    Task<Character> EquipWeaponAsync(Guid characterId, int itemId, bool offHand = false);
    Task<Character> EquipArmorAsync(Guid characterId, int itemId);
    Task<Character> EquipShieldAsync(Guid characterId, int itemId);
  }

  public class EquipmentService(ICharacterRepository characterRepository, IItemRepository itemRepository)
    : IEquipmentService {
    public async Task<Character> EquipWeaponAsync(Guid characterId, int itemId, bool offhand = false) {
      // Check that the item exists
      var item = await itemRepository.GetByIdOrThrowAsync(itemId);

      // Item must be a weapon
      if (!item.IsWeapon()) {
        throw new EquipmentTypeMismatchException(itemId, EquipmentType.Weapon);
      }

      // check that the character exists
      var character = await characterRepository.GetByIdOrThrowAsync(characterId);

      // Possible Scenarios:
      // 1. Two-handed weapon: Equip it in the main hand and unequip any off-hand item
      // 2. One-handed weapon: Equip it in the main hand or off-hand (depending on the offhand flag)
      //    2.1: Player has a 2 handed weapon equipped: Unequip it and equip the new one
      Item? mainHandItem = character.Equipment.MainHand;
      Item? offHandItem = character.Equipment.OffHand;
      if (item.IsTwoHandedWeapon()) {
        // If the weapon is two-handed, unequip any currently equipped weapons or shields
        mainHandItem = item;
        offHandItem = null;
      } else if (mainHandItem != null) {
        // If the player has a two-handed weapon equipped, unequip it
        if (mainHandItem.IsTwoHandedWeapon()) {
          mainHandItem = null;
          offHandItem = null;
        }

        // One-handed weapon: Equip it in the main hand or off-hand (depending on the offhand flag)
        if (!offhand) {
          mainHandItem = item;
        } else {
          offHandItem = item;
        }
      }

      // Update the character's equipment
      character.Equipment.MainHand = mainHandItem;
      character.Equipment.OffHand = offHandItem;
      await characterRepository.UpdateAsync(character);
      return character;
    }

    public async Task<Character> EquipArmorAsync(Guid characterId, int itemId) {
      // Check that the item exists
      var item = await itemRepository.GetByIdOrThrowAsync(itemId);

      // Item must be an armor
      if (!item.IsArmor()) {
        throw new EquipmentTypeMismatchException(itemId, EquipmentType.Armor);
      }

      // check that the character exists
      var character = await characterRepository.GetByIdOrThrowAsync(characterId);


      character.Equipment.Armor = item;
      await characterRepository.UpdateAsync(character);
      return character;
    }

    public async Task<Character> EquipShieldAsync(Guid characterId, int itemId) {
      // Check that the item exists
      var item = await itemRepository.GetByIdOrThrowAsync(itemId);

      // Item must be a shield
      if (!item.IsShield()) {
        throw new EquipmentTypeMismatchException(itemId, EquipmentType.Shield);
      }

      // check that the character exists
      var character = await characterRepository.GetByIdOrThrowAsync(characterId);

      // If the character has a two-handed weapon equipped, unequip it
      var mainHandItem = character.Equipment.MainHand;
      if (mainHandItem?.IsTwoHandedWeapon() ?? false) {
        character.Equipment.MainHand = null;
      }

      // Equip the shield in the off-hand
      character.Equipment.OffHand = item;
      await characterRepository.UpdateAsync(character);
      return character;
    }
  }
}
