using Gearbox.Domain.Entities;
using Gearbox.Domain.Interfaces;
using Gearbox.Infrastructure.Data;

namespace Gearbox.Infrastructure.Repositories
{
    public class PurchaseInvoiceItemRepository : GenericRepository<PurchaseInvoiceItem>, IPurchaseInvoiceItemRepository
    {
        public PurchaseInvoiceItemRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
