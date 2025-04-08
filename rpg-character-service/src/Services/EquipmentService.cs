using RPGCharacterService.Exceptions.Items;
using RPGCharacterService.Models.Characters;
using RPGCharacterService.Models.Items;
using RPGCharacterService.Persistence.Characters;
using RPGCharacterService.Persistence.Items;

namespace RPGCharacterService.Services {
  /// <summary>
  /// Defines the interface for equipment management services.
  /// </summary>
  public interface IEquipmentService {
    /// <summary>
    /// Equips a weapon to a character, either in the main hand or off-hand.
    /// </summary>
    /// <param name="characterId">The unique identifier of the character.</param>
    /// <param name="itemId">The unique identifier of the weapon to equip.</param>
    /// <param name="offHand">Whether to equip the weapon in the off-hand slot.</param>
    /// <returns>The updated character with the new equipment configuration.</returns>
    /// <exception cref="EquipmentTypeMismatchException">Thrown when the item is not a weapon.</exception>
    /// <exception cref="CharacterNotFoundException">Thrown when no character exists with the specified ID.</exception>
    Task<Character> EquipWeaponAsync(Guid characterId, int itemId, bool offHand = false);

    /// <summary>
    /// Equips armor to a character.
    /// </summary>
    /// <param name="characterId">The unique identifier of the character.</param>
    /// <param name="itemId">The unique identifier of the armor to equip.</param>
    /// <returns>The updated character with the new equipment configuration.</returns>
    /// <exception cref="EquipmentTypeMismatchException">Thrown when the item is not armor.</exception>
    /// <exception cref="CharacterNotFoundException">Thrown when no character exists with the specified ID.</exception>
    Task<Character> EquipArmorAsync(Guid characterId, int itemId);

    /// <summary>
    /// Equips a shield to a character.
    /// </summary>
    /// <param name="characterId">The unique identifier of the character.</param>
    /// <param name="itemId">The unique identifier of the shield to equip.</param>
    /// <returns>The updated character with the new equipment configuration.</returns>
    /// <exception cref="EquipmentTypeMismatchException">Thrown when the item is not a shield.</exception>
    /// <exception cref="CharacterNotFoundException">Thrown when no character exists with the specified ID.</exception>
    Task<Character> EquipShieldAsync(Guid characterId, int itemId);
  }

  /// <summary>
  /// Provides functionality for managing character equipment.
  /// This service handles equipping weapons, armor, and shields, including validation and equipment slot management.
  /// </summary>
  public class EquipmentService(ICharacterRepository characterRepository, IItemRepository itemRepository)
    : IEquipmentService {
    /// <summary>
    /// Equips a weapon to a character, either in the main hand or off-hand.
    /// </summary>
    /// <param name="characterId">The unique identifier of the character.</param>
    /// <param name="itemId">The unique identifier of the weapon to equip.</param>
    /// <param name="offhand">Whether to equip the weapon in the off-hand slot.</param>
    /// <returns>The updated character with the new equipment configuration.</returns>
    /// <exception cref="EquipmentTypeMismatchException">Thrown when the item is not a weapon.</exception>
    /// <exception cref="CharacterNotFoundException">Thrown when no character exists with the specified ID.</exception>
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

    /// <summary>
    /// Equips armor to a character.
    /// </summary>
    /// <param name="characterId">The unique identifier of the character.</param>
    /// <param name="itemId">The unique identifier of the armor to equip.</param>
    /// <returns>The updated character with the new equipment configuration.</returns>
    /// <exception cref="EquipmentTypeMismatchException">Thrown when the item is not armor.</exception>
    /// <exception cref="CharacterNotFoundException">Thrown when no character exists with the specified ID.</exception>
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

    /// <summary>
    /// Equips a shield to a character.
    /// </summary>
    /// <param name="characterId">The unique identifier of the character.</param>
    /// <param name="itemId">The unique identifier of the shield to equip.</param>
    /// <returns>The updated character with the new equipment configuration.</returns>
    /// <exception cref="EquipmentTypeMismatchException">Thrown when the item is not a shield.</exception>
    /// <exception cref="CharacterNotFoundException">Thrown when no character exists with the specified ID.</exception>
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
