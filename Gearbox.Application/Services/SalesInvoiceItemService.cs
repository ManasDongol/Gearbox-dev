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
        private readonly IUnitOfWork _unitOfWork;

        public SalesInvoiceItemService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SalesInvoiceItemDto>> GetAllAsync()
        {
            var entities = await _unitOfWork.SalesInvoiceItems.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<SalesInvoiceItemDto> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.SalesInvoiceItems.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<SalesInvoiceItemDto> AddAsync(SalesInvoiceItemDto dto)
        {
            var entity = MapToEntity(dto);
            entity.Id = Guid.NewGuid(); // ensuring a new ID
            await _unitOfWork.SalesInvoiceItems.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, SalesInvoiceItemDto dto)
        {
            var entity = await _unitOfWork.SalesInvoiceItems.GetByIdAsync(id);
            if (entity != null)
            {
                // Assign new values from dto
                // entity.SomeProperty = dto.SomeProperty;
                _unitOfWork.SalesInvoiceItems.Update(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.SalesInvoiceItems.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.SalesInvoiceItems.Remove(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        // Manual Mapping Methods
        private SalesInvoiceItemDto MapToDto(SalesInvoiceItem entity)
        {
            if (entity == null) return null;
            return new SalesInvoiceItemDto
            {
                Id = entity.Id,
                // Map other properties here
            };
        }

        private SalesInvoiceItem MapToEntity(SalesInvoiceItemDto dto)
        {
            if (dto == null) return null;
            return new SalesInvoiceItem
            {
                Id = dto.Id,
                // Map other properties here
            };
        }
    }
}
