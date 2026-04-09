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
    public class SalesInvoiceService : ISalesInvoiceService
    {
        private readonly ISalesInvoiceRepository _repository;

        public SalesInvoiceService(ISalesInvoiceRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<SalesInvoiceDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<SalesInvoiceDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<SalesInvoiceDto> AddAsync(SalesInvoiceDto dto)
        {
            var entity = MapToEntity(dto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, SalesInvoiceDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                // Assign new values from dto
                // (In a real scenario, you'd map individual properties)
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

        private SalesInvoiceDto MapToDto(SalesInvoice entity)
        {
            if (entity == null) return null;
            return new SalesInvoiceDto
            {
                Id = entity.Id,
                CustomerId = entity.CustomerId,
                StaffId = entity.StaffId,
                InvoiceNumber = entity.InvoiceNumber,
                TotalAmount = entity.TotalAmount,
                DiscountAmount = entity.DiscountAmount,
                IsLoyaltyDiscountApplied = entity.IsLoyaltyDiscountApplied,
                IsPaid = entity.IsPaid,
                DueDate = entity.DueDate,
                CreatedDate = entity.CreatedDate,
            };
        }

        private SalesInvoice MapToEntity(SalesInvoiceDto dto)
        {
            if (dto == null) return null;
            return new SalesInvoice
            {
                Id = dto.Id,
                CustomerId = dto.CustomerId,
                StaffId = dto.StaffId,
                InvoiceNumber = dto.InvoiceNumber,
                TotalAmount = dto.TotalAmount,
                DiscountAmount = dto.DiscountAmount,
                IsLoyaltyDiscountApplied = dto.IsLoyaltyDiscountApplied,
                IsPaid = dto.IsPaid,
                DueDate = dto.DueDate,
                CreatedDate = dto.CreatedDate,
            };
        }
    }
}
