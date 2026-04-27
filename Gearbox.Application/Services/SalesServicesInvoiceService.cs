using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gearbox.Application.DTOs;
using Gearbox.Application.Interfaces;
using Gearbox.Domain.Entities;
using Gearbox.Domain.Interfaces;

namespace Gearbox.Application.Services
{
    public class SalesServicesInvoiceService : ISalesServicesInvoiceService
    {
        private readonly ISalesServicesInvoiceRepository _repository;
        private readonly IPartRepository _partRepository;

        public SalesServicesInvoiceService(ISalesServicesInvoiceRepository repository, IPartRepository partRepository)
        {
            _repository = repository;
            _partRepository = partRepository;
        }

        public async Task<IEnumerable<SalesServicesInvoiceDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<SalesServicesInvoiceDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<SalesServicesInvoiceDto> AddAsync(SalesServicesInvoiceDto dto)
        {
            var entity = MapToEntity(dto);
            
            // Reduce stock for each item that is a Part
            if (dto.Items != null && dto.Items.Any())
            {
                foreach (var item in dto.Items)
                {
                    if (item.Type == "Part" && item.PartId.HasValue)
                    {
                        var part = await _partRepository.GetByIdAsync(item.PartId.Value);
                        if (part != null)
                        {
                            part.StockQuantity -= item.Quantity;
                            _partRepository.Update(part);
                        }
                    }
                }
            }

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, SalesServicesInvoiceDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                // Assign new values from dto
                _repository.Update(entity);
                await _repository.SaveChangesAsync();
            }
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
                CreatedAt = entity.CreatedAt,
                Items = entity.Items?.Select(i => new SalesServicesInvoiceItemDto
                {
                    Id = i.Id,
                    SalesServicesInvoiceId = i.SalesServicesInvoiceId,
                    PartId = i.PartId,
                    ServiceDetailsId = i.ServiceDetailsId,
                    Type = i.Type,
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
                Id = dto.Id,
                CustomerId = dto.CustomerId,
                StaffId = dto.StaffId,
                AppointmentId = dto.AppointmentId,
                TotalAmount = dto.TotalAmount,
                DiscountAmount = dto.DiscountAmount,
                CreatedAt = dto.CreatedAt,
                Items = dto.Items?.Select(i => new SalesServicesInvoiceItem
                {
                    Id = i.Id,
                    SalesServicesInvoiceId = i.SalesServicesInvoiceId,
                    PartId = i.PartId,
                    ServiceDetailsId = i.ServiceDetailsId,
                    Type = i.Type,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList() ?? new List<SalesServicesInvoiceItem>()
            };
        }
    }
}
