using RPGCharacterService.Exceptions.Character;
using RPGCharacterService.Exceptions.Equipment;
using RPGCharacterService.Exceptions.Items;
using RPGCharacterService.Entities.Characters;
using RPGCharacterService.Entities.Items;
using RPGCharacterService.Persistence;

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
    /// <exception cref="CharacterNotFoundException">Thrown when no character exists with the specified ID.</exception>
    /// <exception cref="ItemNotFoundException">Thrown when no item exists with the specified ID.</exception>
    /// <exception cref="EquipmentTypeMismatchException">Thrown when the item is not a weapon.</exception>
    Task<Character> EquipWeaponAsync(Guid characterId, int itemId, bool offHand = false);

    /// <summary>
    /// Equips armor to a character.
    /// </summary>
    /// <param name="characterId">The unique identifier of the character.</param>
    /// <param name="itemId">The unique identifier of the armor to equip.</param>
    /// <returns>The updated character with the new equipment configuration.</returns>
    /// <exception cref="CharacterNotFoundException">Thrown when no character exists with the specified ID.</exception>
    /// <exception cref="ItemNotFoundException">Thrown when no item exists with the specified ID.</exception>
    /// <exception cref="EquipmentTypeMismatchException">Thrown when the item is not a weapon.</exception>
    Task<Character> EquipArmorAsync(Guid characterId, int itemId);

    /// <summary>
    /// Equips a shield to a character.
    /// </summary>
    /// <param name="characterId">The unique identifier of the character.</param>
    /// <param name="itemId">The unique identifier of the shield to equip.</param>
    /// <returns>The updated character with the new equipment configuration.</returns>
    /// <exception cref="CharacterNotFoundException">Thrown when no character exists with the specified ID.</exception>
    /// <exception cref="ItemNotFoundException">Thrown when no item exists with the specified ID.</exception>
    /// <exception cref="EquipmentTypeMismatchException">Thrown when the item is not a weapon.</exception>
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
    /// <param name="offHand">Whether to equip the weapon in the off-hand slot.</param>
    /// <returns>The updated character with the new equipment configuration.</returns>
    /// <exception cref="CharacterNotFoundException">Thrown when no character exists with the specified ID.</exception>
    /// <exception cref="ItemNotFoundException">Thrown when no item exists with the specified ID.</exception>
    /// <exception cref="EquipmentTypeMismatchException">Thrown when the item is not a weapon.</exception>
    public async Task<Character> EquipWeaponAsync(Guid characterId, int itemId, bool offHand = false) {
      // Load the item and character
      var itemTask = itemRepository.GetByIdOrThrowAsync(itemId);
      var characterTask = characterRepository.GetByIdOrThrowAsync(characterId);
      await Task.WhenAll(itemTask, characterTask);

      // Equip the weapon
      var item = itemTask.Result;
      var character = characterTask.Result;
      character.Equipment.EquipWeapon(item, offHand);

      // Update the character in the repository
      await characterRepository.UpdateAsync(character);
      return character;
    }

    /// <summary>
    /// Equips armor to a character.
    /// </summary>
    /// <param name="characterId">The unique identifier of the character.</param>
    /// <param name="itemId">The unique identifier of the armor to equip.</param>
    /// <returns>The updated character with the new equipment configuration.</returns>
    /// <exception cref="CharacterNotFoundException">Thrown when no character exists with the specified ID.</exception>
    /// <exception cref="ItemNotFoundException">Thrown when no item exists with the specified ID.</exception>
    /// <exception cref="EquipmentTypeMismatchException">Thrown when the item is not a weapon.</exception>
    public async Task<Character> EquipArmorAsync(Guid characterId, int itemId) {
      // Load the item and character
      var itemTask = itemRepository.GetByIdOrThrowAsync(itemId);
      var characterTask = characterRepository.GetByIdOrThrowAsync(characterId);
      await Task.WhenAll(itemTask, characterTask);

      // Equip the armor
      var item = itemTask.Result;
      var character = characterTask.Result;
      character.Equipment.EquipArmor(item);

      // Update the character in the repository
      await characterRepository.UpdateAsync(character);
      return character;
    }

    /// <summary>
    /// Equips a shield to a character.
    /// </summary>
    /// <param name="characterId">The unique identifier of the character.</param>
    /// <param name="itemId">The unique identifier of the shield to equip.</param>
    /// <returns>The updated character with the new equipment configuration.</returns>
    /// <exception cref="CharacterNotFoundException">Thrown when no character exists with the specified ID.</exception>
    /// <exception cref="ItemNotFoundException">Thrown when no item exists with the specified ID.</exception>
    /// <exception cref="EquipmentTypeMismatchException">Thrown when the item is not a weapon.</exception>
    public async Task<Character> EquipShieldAsync(Guid characterId, int itemId) {
      // Load the item and character
      var itemTask = itemRepository.GetByIdOrThrowAsync(itemId);
      var characterTask = characterRepository.GetByIdOrThrowAsync(characterId);
      await Task.WhenAll(itemTask, characterTask);

      // Equip the shield
      var item = itemTask.Result;
      var character = characterTask.Result;
      character.Equipment.EquipShield(item);

      // Update the character in the repository
      await characterRepository.UpdateAsync(character);
      return character;
    }
  }
}
