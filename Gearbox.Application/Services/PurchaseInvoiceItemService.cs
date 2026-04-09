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
    public class PurchaseInvoiceItemService : IPurchaseInvoiceItemService
    {
        private readonly IPurchaseInvoiceItemRepository _repository;

        public PurchaseInvoiceItemService(IPurchaseInvoiceItemRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PurchaseInvoiceItemDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<PurchaseInvoiceItemDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<PurchaseInvoiceItemDto> AddAsync(PurchaseInvoiceItemDto dto)
        {
            var entity = MapToEntity(dto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, PurchaseInvoiceItemDto dto)
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

        private PurchaseInvoiceItemDto MapToDto(PurchaseInvoiceItem entity)
        {
            if (entity == null) return null;
            return new PurchaseInvoiceItemDto
            {
                Id = entity.Id,
                PurchaseInvoiceId = entity.PurchaseInvoiceId,
                PartId = entity.PartId,
                Quantity = entity.Quantity,
                CostPrice = entity.CostPrice,
            };
        }

        private PurchaseInvoiceItem MapToEntity(PurchaseInvoiceItemDto dto)
        {
            if (dto == null) return null;
            return new PurchaseInvoiceItem
            {
                Id = dto.Id,
                PurchaseInvoiceId = dto.PurchaseInvoiceId,
                PartId = dto.PartId,
                Quantity = dto.Quantity,
                CostPrice = dto.CostPrice,
            };
        }
    }
}
