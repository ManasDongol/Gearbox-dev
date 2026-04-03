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
        private readonly IUnitOfWork _unitOfWork;

        public PurchaseInvoiceItemService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PurchaseInvoiceItemDto>> GetAllAsync()
        {
            var entities = await _unitOfWork.PurchaseInvoiceItems.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<PurchaseInvoiceItemDto> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.PurchaseInvoiceItems.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<PurchaseInvoiceItemDto> AddAsync(PurchaseInvoiceItemDto dto)
        {
            var entity = MapToEntity(dto);
            entity.Id = Guid.NewGuid(); // ensuring a new ID
            await _unitOfWork.PurchaseInvoiceItems.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, PurchaseInvoiceItemDto dto)
        {
            var entity = await _unitOfWork.PurchaseInvoiceItems.GetByIdAsync(id);
            if (entity != null)
            {
                // Assign new values from dto
                // entity.SomeProperty = dto.SomeProperty;
                _unitOfWork.PurchaseInvoiceItems.Update(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.PurchaseInvoiceItems.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.PurchaseInvoiceItems.Remove(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        // Manual Mapping Methods
        private PurchaseInvoiceItemDto MapToDto(PurchaseInvoiceItem entity)
        {
            if (entity == null) return null;
            return new PurchaseInvoiceItemDto
            {
                Id = entity.Id,
                // Map other properties here
            };
        }

        private PurchaseInvoiceItem MapToEntity(PurchaseInvoiceItemDto dto)
        {
            if (dto == null) return null;
            return new PurchaseInvoiceItem
            {
                Id = dto.Id,
                // Map other properties here
            };
        }
    }
}
