using RPGCharacterService.Models.Characters;

namespace RPGCharacterService.Persistence.Characters
{
    public interface ICharacterRepository
    {
        Task<IEnumerable<Character>> GetAllAsync();
        Task<Character?> GetByIdAsync(Guid id);
        Task<Character?> GetByNameAsync(string name);
        Task AddAsync(Character character);
        Task UpdateAsync(Character character);
        Task DeleteAsync(Guid id);
    }
    
} 
