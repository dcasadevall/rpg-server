using RPGCharacterService.Dtos.Character.Requests;
using RPGCharacterService.Models;
using RPGCharacterService.Models.Characters;
using RPGCharacterService.Persistence;
using RPGCharacterService.Exceptions.Character;

namespace RPGCharacterService.Services
{
    public interface ICharacterService
    {
        Task<IEnumerable<Character>> GetAllCharactersAsync();
        Task<Character> GetCharacterAsync(Guid characterId);
        Task<Character> CreateCharacterAsync(CreateCharacterRequest request);
        Task DeleteCharacterAsync(Guid characterId);
    }

    public class CharacterService(
        ICharacterRepository repository,
        IDiceService diceService,
        ICharacterRules characterRules) : ICharacterService
    {
        public async Task<IEnumerable<Character>> GetAllCharactersAsync()
        {
            return await repository.GetAllAsync();
        }

        public async Task<Character> GetCharacterAsync(Guid characterId)
        {
            var character = await repository.GetByIdAsync(characterId);
            if (character == null)
            {
                throw new CharacterNotFoundException(characterId);
            }
            
            return character;
        }

        public async Task<Character> CreateCharacterAsync(CreateCharacterRequest request)
        {
            // NOTE: For the scope of this prompt, we are not using transactions.
            // In a production environment these kind of checks before creation need to be atomic,
            // and would require a transaction to be created and used for both GetByNameAsync and AddAsync.
            var existingCharacter = await repository.GetByNameAsync(request.Name);
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

            await repository.AddAsync(character);
            return character;
        }

        public async Task DeleteCharacterAsync(Guid characterId)
        {
            var character = await repository.GetByIdAsync(characterId);
            if (character == null)
            {
                throw new CharacterNotFoundException(characterId);
            }

            await repository.DeleteAsync(characterId);
        }
    }
}
