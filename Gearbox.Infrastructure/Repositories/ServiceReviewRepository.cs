using Gearbox.Domain.Entities;
using Gearbox.Domain.Interfaces;
using Gearbox.Infrastructure.Data;

namespace Gearbox.Infrastructure.Repositories
{
    public class ServiceReviewRepository : GenericRepository<ServiceReview>, IServiceReviewRepository
    {
        public ServiceReviewRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
