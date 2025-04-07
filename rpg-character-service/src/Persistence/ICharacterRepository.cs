using RPGCharacterService.Models;
using RPGCharacterService.Models.Characters;

namespace RPGCharacterService.Persistence
{
    public interface ICharacterRepository
    {
        List<Character> GetAll();
        Character? GetById(Guid id);
        void Add(Character character);
        void Update(Character character);
        void Delete(Guid id);
    }
    
} 
