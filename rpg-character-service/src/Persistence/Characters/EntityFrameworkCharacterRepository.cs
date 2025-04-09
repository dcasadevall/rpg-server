using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RPGCharacterService.Exceptions.Character;
using RPGCharacterService.Infrastructure.Data;
using RPGCharacterService.Infrastructure.Data.Entities;
using RPGCharacterService.Models.Characters;

namespace RPGCharacterService.Persistence.Characters
{
    /// <summary>
    /// Entity Framework Core implementation of the character repository.
    /// </summary>
    public class EntityFrameworkCharacterRepository(RpgDbContext dbContext, IMapper mapper) : ICharacterRepository
    {
        /// <inheritdoc />
        public async Task<IEnumerable<Character>> GetAllAsync()
        {
            var entities = await dbContext.Characters
                .Include(c => c.AbilityScores)
                .Include(c => c.Currency)
                .Include(c => c.Equipment)
                    .ThenInclude(e => e.MainHandItem)
                        .ThenInclude(i => i != null ? i.WeaponStats : null)
                .Include(c => c.Equipment)
                    .ThenInclude(e => e.OffHandItem)
                        .ThenInclude(i => i != null ? i.WeaponStats : null)
                .Include(c => c.Equipment)
                    .ThenInclude(e => e.ArmorItem)
                        .ThenInclude(i => i != null ? i.ArmorStats : null)
                .ToListAsync();

            return mapper.Map<IEnumerable<Character>>(entities);
        }

        /// <inheritdoc />
        public async Task<Character?> GetByIdAsync(Guid id)
        {
            var entity = await dbContext.Characters
                .Include(c => c.AbilityScores)
                .Include(c => c.Currency)
                .Include(c => c.Equipment)
                    .ThenInclude(e => e.MainHandItem)
                        .ThenInclude(i => i != null ? i.WeaponStats : null)
                .Include(c => c.Equipment)
                    .ThenInclude(e => e.OffHandItem)
                        .ThenInclude(i => i != null ? i.WeaponStats : null)
                .Include(c => c.Equipment)
                    .ThenInclude(e => e.ArmorItem)
                        .ThenInclude(i => i != null ? i.ArmorStats : null)
                .FirstOrDefaultAsync(c => c.Id == id);

            return entity != null ? mapper.Map<Character>(entity) : null;
        }

        /// <inheritdoc />
        public async Task<Character?> GetByNameAsync(string name)
        {
            var entity = await dbContext.Characters
                .Include(c => c.AbilityScores)
                .Include(c => c.Currency)
                .Include(c => c.Equipment)
                    .ThenInclude(e => e.MainHandItem)
                        .ThenInclude(i => i != null ? i.WeaponStats : null)
                .Include(c => c.Equipment)
                    .ThenInclude(e => e.OffHandItem)
                        .ThenInclude(i => i != null ? i.WeaponStats : null)
                .Include(c => c.Equipment)
                    .ThenInclude(e => e.ArmorItem)
                        .ThenInclude(i => i != null ? i.ArmorStats : null)
                .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());

            return entity != null ? mapper.Map<Character>(entity) : null;
        }

        /// <inheritdoc />
        public async Task AddAsync(Character character)
        {
            var entity = mapper.Map<CharacterEntity>(character);
            await dbContext.Characters.AddAsync(entity);
            await dbContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task UpdateAsync(Character character)
        {
            var existingEntity = await dbContext.Characters
                .Include(c => c.AbilityScores)
                .Include(c => c.Currency)
                .Include(c => c.Equipment)
                .FirstOrDefaultAsync(c => c.Id == character.Id);

            if (existingEntity == null)
            {
                throw new CharacterNotFoundException(character.Id);
            }

            mapper.Map(character, existingEntity);
            await dbContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Guid id)
        {
            var entity = await dbContext.Characters.FindAsync(id);
            if (entity == null)
            {
                throw new CharacterNotFoundException(id);
            }

            dbContext.Characters.Remove(entity);
            await dbContext.SaveChangesAsync();
        }
    }
}
