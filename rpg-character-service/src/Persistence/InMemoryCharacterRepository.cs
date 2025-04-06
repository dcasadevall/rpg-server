using RPGCharacterService.Models;

namespace RPGCharacterService.Persistence
{
    public class InMemoryCharacterRepository : ICharacterRepository
    {
        private readonly Dictionary<Guid, Character> characters = new();

        public List<Character> GetAll()
        {
            return characters.Values.ToList();
        }

        public Character? GetById(Guid id)
        {
            if (characters.TryGetValue(id, out var character))
            {
                return character;
            }

            return null;
        }

        public void Add(Character character)
        {
            if (character.Id == Guid.Empty)
            {
                character.Id = Guid.NewGuid();
            }

            characters[character.Id] = character;
        }

        public void Update(Character character)
        {
            if (!characters.ContainsKey(character.Id))
            {
                throw new KeyNotFoundException($"Character with ID {character.Id} not found");
            }

            characters[character.Id] = character;
        }

        public void Delete(Guid id)
        {
            if (!characters.ContainsKey(id))
            {
                throw new KeyNotFoundException($"Character with ID {id} not found");
            }

            characters.Remove(id);
        }
    }
}
