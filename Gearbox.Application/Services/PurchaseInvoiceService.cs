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
        private readonly IUnitOfWork _unitOfWork;

        public PurchaseInvoiceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PurchaseInvoiceDto>> GetAllAsync()
        {
            var entities = await _unitOfWork.PurchaseInvoices.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<PurchaseInvoiceDto> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.PurchaseInvoices.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<PurchaseInvoiceDto> AddAsync(PurchaseInvoiceDto dto)
        {
            var entity = MapToEntity(dto);
            entity.Id = Guid.NewGuid(); // ensuring a new ID
            await _unitOfWork.PurchaseInvoices.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, PurchaseInvoiceDto dto)
        {
            var entity = await _unitOfWork.PurchaseInvoices.GetByIdAsync(id);
            if (entity != null)
            {
                // Assign new values from dto
                // entity.SomeProperty = dto.SomeProperty;
                _unitOfWork.PurchaseInvoices.Update(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.PurchaseInvoices.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.PurchaseInvoices.Remove(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        // Manual Mapping Methods
        private PurchaseInvoiceDto MapToDto(PurchaseInvoice entity)
        {
            if (entity == null) return null;
            return new PurchaseInvoiceDto
            {
                Id = entity.Id,
                // Map other properties here
            };
        }

        private PurchaseInvoice MapToEntity(PurchaseInvoiceDto dto)
        {
            if (dto == null) return null;
            return new PurchaseInvoice
            {
                Id = dto.Id,
                // Map other properties here
            };
        }
    }
}
