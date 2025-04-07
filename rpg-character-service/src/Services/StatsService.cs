using RPGCharacterService.Models;
using RPGCharacterService.Models.Characters;
using RPGCharacterService.Persistence;

namespace RPGCharacterService.Services
{
    public interface IStatsService
    {
        Character ModifyHitPoints(Guid characterId, int delta);
    }

    public class StatsService(ICharacterRepository repository, ICharacterRules characterRules) : IStatsService
    {
        public Character ModifyHitPoints(Guid characterId, int delta)
        {
            var character = repository.GetById(characterId);
            if (character == null)
            {
                throw new KeyNotFoundException($"Character with ID {characterId} not found");
            }

            var maxHitPoints = characterRules.CalculateMaxHitPoints(character);
            character.HitPoints = Math.Clamp(character.HitPoints + delta, 0, maxHitPoints);
            repository.Update(character);

            return character;
        }
    }
} 
