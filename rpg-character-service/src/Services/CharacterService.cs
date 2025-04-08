using RPGCharacterService.Dtos.Character.Requests;
using RPGCharacterService.Models;
using RPGCharacterService.Models.Characters;
using RPGCharacterService.Persistence;
using RPGCharacterService.Exceptions.Character;

namespace RPGCharacterService.Services
{
    public interface ICharacterService
    {
        IEnumerable<Character> GetAllCharacters();
        Character GetCharacter(Guid characterId);
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

        public Character GetCharacter(Guid characterId)
        {
            var character = repository.GetById(characterId);
            if (character == null)
            {
                throw new CharacterNotFoundException(characterId);
            }
            
            return character;
        }

        public Character CreateCharacter(CreateCharacterRequest request)
        {
            // Check if character with same name exists
            // TODO: Transaction
            var existingCharacter = repository.GetByName(request.Name);
            if (existingCharacter != null)
            {
                throw new CharacterAlreadyExistsException(request.Name);
            }

            // First, roll for all ability scores
            var abilityScores = new Dictionary<AbilityScore, int>();
            foreach (var stat in Enum.GetValues<AbilityScore>())
            {
                // Roll 4d6 and take the highest 3
                abilityScores[stat] = diceService
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
                AbilityScores = abilityScores,
            };
            character.HitPoints = characterRules.CalculateMaxHitPoints(character);

            repository.AddAsync(character);
            return character;
        }

        public void DeleteCharacter(Guid characterId)
        {
            var character = repository.GetById(characterId);
            if (character == null)
            {
                throw new CharacterNotFoundException(characterId);
            }

            repository.Delete(characterId);
        }
    }
}
