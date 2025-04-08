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
    public async Task<Character> EquipWeaponAsync(Guid characterId, int itemId, bool offHand = false) {
      // Load the item and character
      var itemTask = itemRepository.GetByIdOrThrowAsync(itemId);
      var characterTask = characterRepository.GetByIdOrThrowAsync(characterId);
      var (item, character) = await Task.WhenAll(itemTask, characterTask);

      character.Equipment.EquipWeapon(item, offHand);
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
      // Load the item and character
      var itemTask = itemRepository.GetByIdOrThrowAsync(itemId);
      var characterTask = characterRepository.GetByIdOrThrowAsync(characterId);
      var (item, character) = await Task.WhenAll(itemTask, characterTask);

      character.Equipment.EquipArmor(item);
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
      // Load the item and character
      var itemTask = itemRepository.GetByIdOrThrowAsync(itemId);
      var characterTask = characterRepository.GetByIdOrThrowAsync(characterId);
      var (item, character) = await Task.WhenAll(itemTask, characterTask);

      character.Equipment.EquipShield(item);
      await characterRepository.UpdateAsync(character);
      return character;
    }
  }
}
