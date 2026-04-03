using Gearbox.Domain.Entities;
using Gearbox.Domain.Interfaces;
using Gearbox.Infrastructure.Data;

namespace Gearbox.Infrastructure.Repositories
{
    public class SalesInvoiceItemRepository : GenericRepository<SalesInvoiceItem>, ISalesInvoiceItemRepository
    {
        public SalesInvoiceItemRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
