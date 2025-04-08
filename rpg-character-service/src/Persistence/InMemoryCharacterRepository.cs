using RPGCharacterService.Models;
using RPGCharacterService.Models.Characters;

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
            return characters.GetValueOrDefault(id);
        }
        
        public Character? GetByName(string name)
        {
            return characters.Values.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public void Add(Character character)
        {
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
