using RPGCharacterService.Exceptions.Character;
using RPGCharacterService.Models.Characters;
using RPGCharacterService.Persistence;

namespace RPGCharacterService.Services
{
    public interface IStatsService
    {
        Task<Character> ModifyHitPointsAsync(Guid characterId, int delta);
    }

    public class StatsService(ICharacterRepository repository, ICharacterRules characterRules) : IStatsService
    {
        public async Task<Character> ModifyHitPointsAsync(Guid characterId, int delta)
        {
            var character = await repository.GetByIdAsync(characterId);
            if (character == null)
            {
                throw new CharacterNotFoundException(characterId);
            }

            var maxHitPoints = characterRules.CalculateMaxHitPoints(character);
            character.HitPoints = Math.Clamp(character.HitPoints + delta, 0, maxHitPoints);
            await repository.UpdateAsync(character);

            return character;
        }
    }
} 
