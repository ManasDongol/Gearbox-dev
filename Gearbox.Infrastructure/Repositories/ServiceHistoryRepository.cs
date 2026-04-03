using Gearbox.Domain.Entities;
using Gearbox.Domain.Interfaces;
using Gearbox.Infrastructure.Data;

namespace Gearbox.Infrastructure.Repositories
{
    public class ServiceHistoryRepository : GenericRepository<ServiceHistory>, IServiceHistoryRepository
    {
        public ServiceHistoryRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
