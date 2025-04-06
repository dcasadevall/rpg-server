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

    public class CharacterService(ICharacterRepository repository) : ICharacterService
    {
        public IEnumerable<Character> GetAllCharacters()
        {
            return repository.GetAll();
        }
        
        public Character GetCharacterById(Guid characterId)
        {
            var character = repository.GetById(characterId);
            if (character == null)
                throw new KeyNotFoundException($"Character with ID {characterId} not found");

            return character;
        }

        public Character CreateCharacter(CreateCharacterRequest request)
        {
            // TODO: Actual character creation logic
            var character = new Character
            {
                Name = request.Name ?? "Unknown",
                Race = request.Race,
                Subrace = request?.Subrace ?? "",
                Class = request.Class,
                HitPoints = 10,
                MaxHitPoints = 10,
                Stats = new Dictionary<StatType, int>
                {
                    {StatType.Strength, 10},
                    {StatType.Dexterity, 10},
                    {StatType.Constitution, 10},
                    {StatType.Intelligence, 10},
                    {StatType.Wisdom, 10},
                    {StatType.Charisma, 10}
                },
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
