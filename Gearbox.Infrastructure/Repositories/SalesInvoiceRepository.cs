using Gearbox.Domain.Entities;
using Gearbox.Domain.Interfaces;
using Gearbox.Infrastructure.Data;

namespace Gearbox.Infrastructure.Repositories
{
    public class SalesInvoiceRepository : GenericRepository<SalesServicesInvoice>, ISalesInvoiceRepository
    {
        public SalesInvoiceRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
