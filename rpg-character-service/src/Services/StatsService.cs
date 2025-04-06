using RPGCharacterService.Models;
using RPGCharacterService.Persistence;

namespace RPGCharacterService.Services
{
    public interface IStatsService
    {
        Character ModifyHitPoints(Guid characterId, int delta);
    }

    public class StatsService(ICharacterRepository repository, IDiceService diceService) : IStatsService
    {
        public Character ModifyHitPoints(Guid characterId, int delta)
        {
            var character = repository.GetById(characterId);
            if (character == null)
            {
                throw new KeyNotFoundException($"Character with ID {characterId} not found");
            }

            character.HitPoints = Math.Clamp(character.HitPoints + delta, 0, character.MaxHitPoints);
            repository.Update(character);

            return character;
        }
    }
} 
