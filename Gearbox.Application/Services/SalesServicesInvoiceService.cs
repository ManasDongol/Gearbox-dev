using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gearbox.Application.BackgroundJobs;
using Gearbox.Application.DTOs;
using Gearbox.Application.Interfaces;
using Gearbox.Domain.Entities;
using Gearbox.Domain.Interfaces;
using Gearbox.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Gearbox.Application.Services
{
    public class SalesServicesInvoiceService : ISalesServicesInvoiceService
    {
        private readonly ISalesServicesInvoiceRepository _repository;
        private readonly IPartRepository _partRepository;
        private readonly ApplicationDbContext _context;
        private readonly EmailQueue _emailQueue;
        private readonly INotificationService _notificationService;

        public SalesServicesInvoiceService(
            ISalesServicesInvoiceRepository repository,
            IPartRepository partRepository,
            ApplicationDbContext context,
            EmailQueue emailQueue,
            INotificationService notificationService)
        {
            _repository = repository;
            _partRepository = partRepository;
            _context = context;
            _emailQueue = emailQueue;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<SalesServicesInvoiceDto>> GetAllAsync()
        {
            var entities = await _context.SalesServicesInvoices
                .AsNoTracking()
                .Include(i => i.Items)
                    .ThenInclude(i => i.Part)
                .Include(i => i.Items)
                    .ThenInclude(i => i.Service)
                .ToListAsync();

            return entities.Select(e => MapToDto(e));
        }

        public async Task<SalesServicesInvoiceDto> GetByIdAsync(Guid id)
        {
            var entity = await _context.SalesServicesInvoices
                .AsNoTracking()
                .Include(i => i.Items)
                    .ThenInclude(i => i.Part)
                .Include(i => i.Items)
                    .ThenInclude(i => i.Service)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (entity == null) return null;
            return MapToDto(entity);
        }

       public async Task<SalesServicesInvoiceDto> AddAsync(NewSalesServicesInvoiceDto dto)
{
    if (dto.Items == null || !dto.Items.Any())
        throw new ArgumentException("Invoice must contain at least one item.");

    // Validate items
    foreach (var item in dto.Items)
    {
        if (item.PartId.HasValue && item.ServiceId.HasValue)
            throw new ArgumentException("Each item must have either PartId or ServiceId, not both.");

        if (!item.PartId.HasValue && !item.ServiceId.HasValue)
            throw new ArgumentException("Each item must have either PartId or ServiceId.");

        if (item.Type == "Service" && !item.VehicleId.HasValue)
            throw new ArgumentException("Each service item must have a vehicle.");

        if (item.Quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0.");

        if (item.UnitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative.");
    }

    var items = new List<SalesServicesInvoiceItem>();
    var serviceHistoryItems = new List<NewSalesServicesInvoiceItemDto>();

    foreach (var item in dto.Items)
    {
        // Handle stock for parts
        if (item.Type == "Part" && item.PartId.HasValue)
        {
            var part = await _partRepository.GetByIdAsync(item.PartId.Value);

            if (part == null)
                throw new Exception($"Part with ID {item.PartId} not found.");

            if (part.StockQuantity < item.Quantity)
                throw new Exception($"Insufficient stock for part {part.Name}");

            part.StockQuantity -= item.Quantity;
            _partRepository.Update(part);
        }

        if (item.Type == "Service" && item.ServiceId.HasValue && item.VehicleId.HasValue)
        {
            var serviceExists = await _context.Services.AnyAsync(s => s.Id == item.ServiceId.Value);
            if (!serviceExists)
                throw new Exception($"Service with ID {item.ServiceId} not found.");

            var vehicleBelongsToCustomer = await _context.Vehicles.AnyAsync(v =>
                v.Id == item.VehicleId.Value && v.CustomerId == dto.CustomerId);

            if (!vehicleBelongsToCustomer)
                throw new Exception("Selected vehicle does not belong to the invoice customer.");

            serviceHistoryItems.Add(item);
        }

        items.Add(new SalesServicesInvoiceItem
        {
            Id = Guid.NewGuid(),
            PartId = item.PartId,
            ServiceId = item.ServiceId,
            Type = item.Type,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice
        });
    }

    // Calculate total safely
    var totalAmount = items.Sum(i => i.Quantity * i.UnitPrice);
    var discountAmount = totalAmount > 5000 ? totalAmount * 0.10m : 0;
    var finalAmount = totalAmount - discountAmount;

    // Create invoice
    var invoice = new SalesServicesInvoice
    {
        Id = Guid.NewGuid(),
        CustomerId = dto.CustomerId,
        StaffId = dto.StaffId,
        AppointmentId = dto.AppointmentId,
        DiscountAmount = discountAmount,
        TotalAmount = finalAmount,
        CreatedAt = DateTime.UtcNow,
        Items = items
    };

    foreach (var serviceItem in serviceHistoryItems)
    {
        await _context.ServiceHistories.AddAsync(new ServiceHistory
        {
            Id = Guid.NewGuid(),
            CustomerId = dto.CustomerId,
            VehicleId = serviceItem.VehicleId!.Value,
            ServiceId = serviceItem.ServiceId!.Value,
            ServiceDate = DateTime.UtcNow,
            Notes = $"Created from sales invoice {invoice.Id}",
            TotalCost = serviceItem.Quantity * serviceItem.UnitPrice
        });
    }

    // Save everything in one go
    await _repository.AddAsync(invoice);
    await _repository.SaveChangesAsync();
    await RecalculateCustomerInvoiceTotalsAsync(invoice.CustomerId);
    await _repository.SaveChangesAsync();

    await CheckInventoryAndNotifyAsync();
    await SendUnpaidCreditRemindersAsync();

    QueueInvoiceEmail(invoice);

    return MapToDto(invoice);
}
        public async Task UpdateAsync(Guid id, SalesServicesInvoiceDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                var originalCustomerId = entity.CustomerId;
                entity.CustomerId = dto.CustomerId;
                entity.StaffId = dto.StaffId;
                entity.AppointmentId = dto.AppointmentId;
                entity.TotalAmount = dto.TotalAmount;
                entity.DiscountAmount = dto.DiscountAmount;
                entity.PaymentStatus = dto.PaymentStatus;
                // Note: Updating items and restoring/reducing stock would be more complex and usually requires a business decision on how to handle it.
                // For now, updating basic fields.
                
                _repository.Update(entity);
                await _repository.SaveChangesAsync();
                await RecalculateCustomerInvoiceTotalsAsync(originalCustomerId);
                if (originalCustomerId != entity.CustomerId)
                {
                    await RecalculateCustomerInvoiceTotalsAsync(entity.CustomerId);
                }
                await _repository.SaveChangesAsync();
            }
        }

        public async Task<SalesServicesInvoiceDto> MarkAsPaidAsync(Guid id)
        {
            var entity = await _context.SalesServicesInvoices
                .Include(i => i.Items)
                    .ThenInclude(i => i.Part)
                .Include(i => i.Items)
                    .ThenInclude(i => i.Service)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (entity == null) return null;

            entity.PaymentStatus = true;
            await _context.SaveChangesAsync();
            await RecalculateCustomerInvoiceTotalsAsync(entity.CustomerId);
            await _context.SaveChangesAsync();

            return MapToDto(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                var customerId = entity.CustomerId;
                _repository.Remove(entity);
                await _repository.SaveChangesAsync();
                await RecalculateCustomerInvoiceTotalsAsync(customerId);
                await _repository.SaveChangesAsync();
            }
        }

        private async Task RecalculateCustomerInvoiceTotalsAsync(Guid customerId)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == customerId);
            if (customer == null) return;

            customer.TotalSpent = await _context.SalesServicesInvoices
                .Where(invoice => invoice.CustomerId == customerId)
                .Select(invoice => (decimal?)invoice.TotalAmount)
                .SumAsync() ?? 0;

            customer.PendingCredits = await _context.SalesServicesInvoices
                .Where(invoice => invoice.CustomerId == customerId && !invoice.PaymentStatus)
                .Select(invoice => (decimal?)invoice.TotalAmount)
                .SumAsync() ?? 0;
        }

        private async Task CheckInventoryAndNotifyAsync()
        {
            var lowStockParts = await _context.Parts.Where(p => p.StockQuantity < 10).ToListAsync();
            foreach (var part in lowStockParts)
            {
                var message = $"Low stock alert: {part.Name} has {part.StockQuantity} remaining.";
                await _notificationService.SendToRoleAsync("Admin", message);
            }
        }

        private async Task SendUnpaidCreditRemindersAsync()
        {
            var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);
            var overdueInvoices = await _context.SalesServicesInvoices
                .Include(i => i.Customer)
                    .ThenInclude(c => c.User)
                .Where(i => !i.PaymentStatus && i.CreatedAt < oneMonthAgo)
                .ToListAsync();

            var overDueCustomers = overdueInvoices.GroupBy(i => i.CustomerId);

            foreach (var customerGroup in overDueCustomers)
            {
                var firstInvoice = customerGroup.First();
                var customer = firstInvoice.Customer;
                if (customer?.User?.Email == null) continue;

                var totalOwed = customerGroup.Sum(i => i.TotalAmount);
                var customerName = $"{customer.User.FirstName} {customer.User.LastName}".Trim();
                if (string.IsNullOrWhiteSpace(customerName))
                {
                    customerName = customer.User.UserName ?? "Customer";
                }

                _emailQueue.Enqueue(new EmailJob
                {
                    ToEmail = customer.User.Email,
                    Subject = "Unpaid Invoice Reminder",
                    Type = EmailType.PaymentReminder,
                    Data = new Dictionary<string, object>
                    {
                        { "customerName", customerName },
                        { "dueAmount", totalOwed }
                    }
                });
            }
        }

        private void QueueInvoiceEmail(SalesServicesInvoice invoice)
        {
            var customer = _context.Customers
                .Include(c => c.User)
                .FirstOrDefault(c => c.UserId == invoice.CustomerId);

            if (string.IsNullOrWhiteSpace(customer?.User?.Email)) return;

            var customerName = $"{customer.User.FirstName} {customer.User.LastName}".Trim();
            if (string.IsNullOrWhiteSpace(customerName))
            {
                customerName = customer.User.UserName ?? "Customer";
            }

            _emailQueue.Enqueue(new EmailJob
            {
                ToEmail = customer.User.Email,
                Subject = $"Gearbox invoice {invoice.Id.ToString()[..8]}",
                Type = EmailType.Invoice,
                Data = new Dictionary<string, object>
                {
                    { "customerName", customerName },
                    { "invoiceId", invoice.Id.ToString()[..8] },
                    { "amount", invoice.TotalAmount }
                }
            });
        }

        private SalesServicesInvoiceDto MapToDto(SalesServicesInvoice entity)
        {
            if (entity == null) return null;
            return new SalesServicesInvoiceDto
            {
                Id = entity.Id,
                CustomerId = entity.CustomerId,
                StaffId = entity.StaffId,
                AppointmentId = entity.AppointmentId,
                TotalAmount = entity.TotalAmount,
                DiscountAmount = entity.DiscountAmount,
                PaymentStatus = entity.PaymentStatus,
                CreatedAt = entity.CreatedAt,
                Items = entity.Items?.Select(i => new SalesServicesInvoiceItemDto
                {
                    Id = i.Id,
                    SalesServicesInvoiceId = i.SalesServicesInvoiceId,
                    PartId = i.PartId,
                    ServiceId = i.ServiceId,
                    VehicleId = null,
                    Type = i.Type,
                    ItemName = i.Part?.Name ?? i.Service?.Name ?? i.Type,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList() ?? new List<SalesServicesInvoiceItemDto>()
            };
        }

        private SalesServicesInvoice MapToEntity(SalesServicesInvoiceDto dto)
        {
            if (dto == null) return null;
            return new SalesServicesInvoice
            {
                Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                CustomerId = dto.CustomerId,
                StaffId = dto.StaffId,
                AppointmentId = dto.AppointmentId,
                TotalAmount = dto.TotalAmount,
                DiscountAmount = dto.DiscountAmount,
                PaymentStatus = dto.PaymentStatus,
                CreatedAt = dto.CreatedAt == default ? DateTime.UtcNow : dto.CreatedAt,
                Items = dto.Items?.Select(i => new SalesServicesInvoiceItem
                {
                    Id = i.Id == Guid.Empty ? Guid.NewGuid() : i.Id,
                    SalesServicesInvoiceId = i.SalesServicesInvoiceId,
                    PartId = i.PartId,
                    ServiceId = i.ServiceId,
                    Type = i.Type,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList() ?? new List<SalesServicesInvoiceItem>()
            };
        }
    }
}
