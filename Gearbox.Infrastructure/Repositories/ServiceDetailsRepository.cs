using Gearbox.Domain.Entities;
using Gearbox.Domain.Interfaces;
using Gearbox.Infrastructure.Data;

namespace Gearbox.Infrastructure.Repositories
{
    public class ServiceDetailsRepository : GenericRepository<ServiceDetails>, IServiceDetailsRepository
    {
        public ServiceDetailsRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
