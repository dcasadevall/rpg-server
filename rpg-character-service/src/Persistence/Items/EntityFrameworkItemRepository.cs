using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RPGCharacterService.Infrastructure.Data;
using RPGCharacterService.Models.Items;

namespace RPGCharacterService.Persistence.Items
{
    /// <summary>
    /// Entity Framework Core implementation of the item repository.
    /// </summary>
    public class EntityFrameworkItemRepository(RpgDbContext dbContext, IMapper mapper) : IItemRepository
    {
        /// <inheritdoc />
        public async Task<Item?> GetByIdAsync(int id)
        {
            var entity = await dbContext.Items
                .Include(i => i.ArmorStats)
                .Include(i => i.WeaponStats)
                .FirstOrDefaultAsync(i => i.Id == id);

            return entity != null ? mapper.Map<Item>(entity) : null;
        }
    }
}
