using Gearbox.Domain.Entities;
using Gearbox.Domain.Interfaces;
using Gearbox.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Gearbox.Infrastructure.Repositories
{
    public class PartRepository : GenericRepository<Part>, IPartRepository
    {
        public PartRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Part>> GetLowStockPartsAsync(int threshold = 10)
        {
            return await _dbSet
                .Where(p => p.StockQuantity < threshold)
                .ToListAsync();
        }
    }
}
