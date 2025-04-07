using RPGCharacterService.Dtos.Character.Requests;
using RPGCharacterService.Models;
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

    public class CharacterService(ICharacterRepository repository, IDiceService diceService) : ICharacterService
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
            
            // At level 1, the character has 10 hit points + Constitution modifier
            // This should be moved to a separate function if we have more than one level
            var maxHp = stats[StatType.Constitution] + 2;

            var character = new Character
            {
                Name = request.Name,
                Race = request.Race,
                Subrace = request?.Subrace ?? "",
                Class = request.Class,
                HitPoints = maxHp,
                MaxHitPoints = maxHp,
                Level = 1,
                Stats = stats,
                Currencies = new Dictionary<CurrencyType, int>()
            };

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
