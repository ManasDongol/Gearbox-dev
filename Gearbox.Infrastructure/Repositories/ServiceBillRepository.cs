using Gearbox.Domain.Entities;
using Gearbox.Domain.Interfaces;
using Gearbox.Infrastructure.Data;

namespace Gearbox.Infrastructure.Repositories
{
    public class ServiceBillRepository : GenericRepository<ServiceBill>, IServiceBillRepository
    {
        public ServiceBillRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
