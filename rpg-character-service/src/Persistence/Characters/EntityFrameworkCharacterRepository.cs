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
    public class EntityFrameworkCharacterRepository : ICharacterRepository
    {
        private readonly RpgDbContext _dbContext;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkCharacterRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        public EntityFrameworkCharacterRepository(RpgDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Character>> GetAllAsync()
        {
            var entities = await _dbContext.Characters
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

            return _mapper.Map<IEnumerable<Character>>(entities);
        }

        /// <inheritdoc />
        public async Task<Character?> GetByIdAsync(Guid id)
        {
            var entity = await _dbContext.Characters
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

            return entity != null ? _mapper.Map<Character>(entity) : null;
        }

        /// <inheritdoc />
        public async Task<Character?> GetByNameAsync(string name)
        {
            var entity = await _dbContext.Characters
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

            return entity != null ? _mapper.Map<Character>(entity) : null;
        }

        /// <inheritdoc />
        public async Task AddAsync(Character character)
        {
            var entity = _mapper.Map<CharacterEntity>(character);
            await _dbContext.Characters.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task UpdateAsync(Character character)
        {
            var existingEntity = await _dbContext.Characters
                .Include(c => c.AbilityScores)
                .Include(c => c.Currency)
                .Include(c => c.Equipment)
                .FirstOrDefaultAsync(c => c.Id == character.Id);

            if (existingEntity == null)
            {
                throw new CharacterNotFoundException(character.Id);
            }

            _mapper.Map(character, existingEntity);
            await _dbContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Guid id)
        {
            var entity = await _dbContext.Characters.FindAsync(id);
            if (entity == null)
            {
                throw new CharacterNotFoundException(id);
            }

            _dbContext.Characters.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
