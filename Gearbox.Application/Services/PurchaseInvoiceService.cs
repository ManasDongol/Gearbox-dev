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
    public class PurchaseInvoiceService : IPurchaseInvoiceService
    {
        private readonly IPurchaseInvoiceRepository _repository;

        public PurchaseInvoiceService(IPurchaseInvoiceRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PurchaseInvoiceDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<PurchaseInvoiceDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<PurchaseInvoiceDto> AddAsync(PurchaseInvoiceDto dto)
        {
            var entity = MapToEntity(dto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, PurchaseInvoiceDto dto)
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

        private PurchaseInvoiceDto MapToDto(PurchaseInvoice entity)
        {
            if (entity == null) return null;
            return new PurchaseInvoiceDto
            {
                Id = entity.Id,
                VendorId = entity.VendorId,
                InvoiceNumber = entity.InvoiceNumber,
                TotalAmount = entity.TotalAmount,
                CreatedDate = entity.CreatedDate,
            };
        }

        private PurchaseInvoice MapToEntity(PurchaseInvoiceDto dto)
        {
            if (dto == null) return null;
            return new PurchaseInvoice
            {
                Id = dto.Id,
                VendorId = dto.VendorId,
                InvoiceNumber = dto.InvoiceNumber,
                TotalAmount = dto.TotalAmount,
                CreatedDate = dto.CreatedDate,
            };
        }
    }
}
