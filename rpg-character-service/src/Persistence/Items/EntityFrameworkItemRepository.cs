using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RPGCharacterService.Infrastructure.Data;
using RPGCharacterService.Models.Items;

namespace RPGCharacterService.Persistence.Items
{
    /// <summary>
    /// Entity Framework Core implementation of the item repository.
    /// </summary>
    public class EntityFrameworkItemRepository : IItemRepository
    {
        private readonly RpgDbContext _dbContext;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkItemRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        public EntityFrameworkItemRepository(RpgDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<Item?> GetByIdAsync(int id)
        {
            var entity = await _dbContext.Items
                .Include(i => i.ArmorStats)
                .Include(i => i.WeaponStats)
                .FirstOrDefaultAsync(i => i.Id == id);

            return entity != null ? _mapper.Map<Item>(entity) : null;
        }
    }
}
