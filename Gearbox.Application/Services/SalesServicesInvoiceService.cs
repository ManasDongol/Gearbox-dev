using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public SalesServicesInvoiceService(
            ISalesServicesInvoiceRepository repository,
            IPartRepository partRepository,
            ApplicationDbContext context)
        {
            _repository = repository;
            _partRepository = partRepository;
            _context = context;
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

        if (item.Quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0.");

        if (item.UnitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative.");
    }

    // Create items first
    var items = new List<SalesServicesInvoiceItem>();

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
    var finalAmount = totalAmount - dto.DiscountAmount;

    // Create invoice
    var invoice = new SalesServicesInvoice
    {
        Id = Guid.NewGuid(),
        CustomerId = dto.CustomerId,
        StaffId = dto.StaffId,
        AppointmentId = dto.AppointmentId,
        DiscountAmount = dto.DiscountAmount,
        TotalAmount = finalAmount,
        CreatedAt = DateTime.UtcNow,
        Items = items
    };

    // Save everything in one go
    await _repository.AddAsync(invoice);
    await _repository.SaveChangesAsync();

    return MapToDto(invoice);
}
        public async Task UpdateAsync(Guid id, SalesServicesInvoiceDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
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

            return MapToDto(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                _repository.Remove(entity);
                await _repository.SaveChangesAsync();
            }
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
