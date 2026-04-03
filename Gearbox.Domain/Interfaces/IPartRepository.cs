using Gearbox.Domain.Entities;

namespace Gearbox.Domain.Interfaces
{
    public interface IPartRepository : IGenericRepository<Part>
    {
        Task<IEnumerable<Part>> GetLowStockPartsAsync(int threshold = 10);
    }
}
