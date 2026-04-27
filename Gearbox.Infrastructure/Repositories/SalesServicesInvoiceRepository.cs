using Gearbox.Domain.Entities;
using Gearbox.Domain.Interfaces;
using Gearbox.Infrastructure.Data;

namespace Gearbox.Infrastructure.Repositories
{
    public class SalesServicesInvoiceRepository : GenericRepository<SalesServicesInvoice>, ISalesServicesInvoiceRepository
    {
        public SalesServicesInvoiceRepository(ApplicationDbContext context) : base(context)
        {
        }
    }

    public class SalesServicesInvoiceItemRepository : GenericRepository<SalesServicesInvoiceItem>, ISalesServicesInvoiceItemRepository
    {
        public SalesServicesInvoiceItemRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
