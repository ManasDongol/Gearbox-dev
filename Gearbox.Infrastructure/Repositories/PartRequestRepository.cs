using Gearbox.Domain.Entities;
using Gearbox.Domain.Interfaces;
using Gearbox.Infrastructure.Data;

namespace Gearbox.Infrastructure.Repositories
{
    public class PartRequestRepository : GenericRepository<PartRequest>, IPartRequestRepository
    {
        public PartRequestRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
