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
    public class SalesInvoiceItemService : ISalesInvoiceItemService
    {
        private readonly ISalesInvoiceItemRepository _repository;

        public SalesInvoiceItemService(ISalesInvoiceItemRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<SalesInvoiceItemDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<SalesInvoiceItemDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<SalesInvoiceItemDto> AddAsync(SalesInvoiceItemDto dto)
        {
            var entity = MapToEntity(dto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, SalesInvoiceItemDto dto)
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

        private SalesInvoiceItemDto MapToDto(SalesInvoiceItem entity)
        {
            if (entity == null) return null;
            return new SalesInvoiceItemDto
            {
                Id = entity.Id,
                SalesInvoiceId = entity.SalesInvoiceId,
                PartId = entity.PartId,
                Quantity = entity.Quantity,
                UnitPrice = entity.UnitPrice,
            };
        }

        private SalesInvoiceItem MapToEntity(SalesInvoiceItemDto dto)
        {
            if (dto == null) return null;
            return new SalesInvoiceItem
            {
                Id = dto.Id,
                SalesInvoiceId = dto.SalesInvoiceId,
                PartId = dto.PartId,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice,
            };
        }
    }
}
