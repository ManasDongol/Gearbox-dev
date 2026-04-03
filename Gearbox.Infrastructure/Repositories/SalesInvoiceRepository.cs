using Gearbox.Domain.Entities;
using Gearbox.Domain.Interfaces;
using Gearbox.Infrastructure.Data;

namespace Gearbox.Infrastructure.Repositories
{
    public class SalesInvoiceRepository : GenericRepository<SalesInvoice>, ISalesInvoiceRepository
    {
        public SalesInvoiceRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
