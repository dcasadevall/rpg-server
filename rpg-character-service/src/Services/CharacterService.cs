using RPGCharacterService.Dtos.Character.Requests;
using RPGCharacterService.Models;
using RPGCharacterService.Models.Characters;
using RPGCharacterService.Persistence;

namespace RPGCharacterService.Services
{
    public interface ICharacterService
    {
        IEnumerable<Character> GetAllCharacters();
        Character GetCharacterById(Guid characterId);
        Character CreateCharacter(CreateCharacterRequest request);
        void DeleteCharacter(Guid characterId);
    }

    public class CharacterService(
        ICharacterRepository repository,
        IDiceService diceService,
        ICharacterRules characterRules) : ICharacterService
    {
        public IEnumerable<Character> GetAllCharacters()
        {
            return repository.GetAll();
        }

        public Character GetCharacterById(Guid characterId)
        {
            var character = repository.GetById(characterId);
            if (character == null)
            {
                throw new KeyNotFoundException($"Character with ID {characterId} not found");
            }

            return character;
        }

        public Character CreateCharacter(CreateCharacterRequest request)
        {
            // First, roll for all stats
            var stats = new Dictionary<StatType, int>();
            foreach (var stat in Enum.GetValues<StatType>())
            {
                // Roll 4d6 and take the highest 3
                stats[stat] = diceService
                              .Roll(DiceSides.Six, 4)
                              .OrderByDescending(x => x)
                              .Take(3)
                              .Sum();
            }

            // Create character and set initial HP based on Max HP
            var character = new Character
            {
                Name = request.Name,
                Race = request.Race,
                Subrace = request?.Subrace ?? "",
                Class = request.Class,
                Level = 1,
                Stats = stats,
            };
            character.HitPoints = characterRules.CalculateMaxHitPoints(character);

            repository.Add(character);
            return character;
        }

        public void DeleteCharacter(Guid characterId)
        {
            var character = repository.GetById(characterId);
            if (character == null)
            {
                throw new KeyNotFoundException($"Character with ID {characterId} not found");
            }

            repository.Delete(characterId);
        }
    }
}
