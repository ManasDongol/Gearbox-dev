using System;
using System.Threading.Tasks;

namespace Gearbox.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        ICustomerRepository Customers { get; }
        IStaffRepository Staffs { get; }
        IVehicleRepository Vehicles { get; }
        IPartRepository Parts { get; }
        IVendorRepository Vendors { get; }
        ISalesInvoiceRepository SalesInvoices { get; }
        IPurchaseInvoiceRepository PurchaseInvoices { get; }
        IAppointmentRepository Appointments { get; }
        IServiceDetailsRepository ServiceDetails { get; }
        IServiceHistoryRepository ServiceHistories { get; }
        IServiceBillRepository ServiceBills { get; }
        IServiceReviewRepository ServiceReviews { get; }
        IPartRequestRepository PartRequests { get; }
        INotificationRepository Notifications { get; }
        IPurchaseInvoiceItemRepository PurchaseInvoiceItems { get; }
        ISalesInvoiceItemRepository SalesInvoiceItems { get; }

        Task<int> CompleteAsync();
    }
}
