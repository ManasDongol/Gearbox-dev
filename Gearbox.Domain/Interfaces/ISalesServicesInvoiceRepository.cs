using Gearbox.Domain.Entities;

namespace Gearbox.Domain.Interfaces
{
    public interface ISalesServicesInvoiceRepository : IGenericRepository<SalesServicesInvoice>
    {
    }

    public interface ISalesServicesInvoiceItemRepository : IGenericRepository<SalesServicesInvoiceItem>
    {
    }
}
