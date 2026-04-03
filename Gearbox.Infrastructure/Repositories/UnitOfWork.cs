using System;
using System.Threading.Tasks;
using Gearbox.Domain.Interfaces;
using Gearbox.Infrastructure.Data;

namespace Gearbox.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IUserRepository Users { get; private set; }
        public ICustomerRepository Customers { get; private set; }
        public IStaffRepository Staffs { get; private set; }
        public IVehicleRepository Vehicles { get; private set; }
        public IPartRepository Parts { get; private set; }
        public IVendorRepository Vendors { get; private set; }
        public ISalesInvoiceRepository SalesInvoices { get; private set; }
        public IPurchaseInvoiceRepository PurchaseInvoices { get; private set; }
        public IAppointmentRepository Appointments { get; private set; }
        public IServiceDetailsRepository ServiceDetails { get; private set; }
        public IServiceHistoryRepository ServiceHistories { get; private set; }
        public IServiceBillRepository ServiceBills { get; private set; }
        public IServiceReviewRepository ServiceReviews { get; private set; }
        public IPartRequestRepository PartRequests { get; private set; }
        public INotificationRepository Notifications { get; private set; }
        public IPurchaseInvoiceItemRepository PurchaseInvoiceItems { get; private set; }
        public ISalesInvoiceItemRepository SalesInvoiceItems { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Users = new UserRepository(context);
            Customers = new CustomerRepository(context);
            Staffs = new StaffRepository(context);
            Vehicles = new VehicleRepository(context);
            Parts = new PartRepository(context);
            Vendors = new VendorRepository(context);
            SalesInvoices = new SalesInvoiceRepository(context);
            PurchaseInvoices = new PurchaseInvoiceRepository(context);
            Appointments = new AppointmentRepository(context);
            ServiceDetails = new ServiceDetailsRepository(context);
            ServiceHistories = new ServiceHistoryRepository(context);
            ServiceBills = new ServiceBillRepository(context);
            ServiceReviews = new ServiceReviewRepository(context);
            PartRequests = new PartRequestRepository(context);
            Notifications = new NotificationRepository(context);
            PurchaseInvoiceItems = new PurchaseInvoiceItemRepository(context);
            SalesInvoiceItems = new SalesInvoiceItemRepository(context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
