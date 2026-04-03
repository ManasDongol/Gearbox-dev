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
        private readonly IUnitOfWork _unitOfWork;

        public SalesInvoiceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SalesInvoiceDto>> GetAllAsync()
        {
            var entities = await _unitOfWork.SalesInvoices.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<SalesInvoiceDto> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.SalesInvoices.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<SalesInvoiceDto> AddAsync(SalesInvoiceDto dto)
        {
            var entity = MapToEntity(dto);
            entity.Id = Guid.NewGuid(); // ensuring a new ID
            await _unitOfWork.SalesInvoices.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, SalesInvoiceDto dto)
        {
            var entity = await _unitOfWork.SalesInvoices.GetByIdAsync(id);
            if (entity != null)
            {
                // Assign new values from dto
                // entity.SomeProperty = dto.SomeProperty;
                _unitOfWork.SalesInvoices.Update(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.SalesInvoices.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.SalesInvoices.Remove(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        // Manual Mapping Methods
        private SalesInvoiceDto MapToDto(SalesInvoice entity)
        {
            if (entity == null) return null;
            return new SalesInvoiceDto
            {
                Id = entity.Id,
                // Map other properties here
            };
        }

        private SalesInvoice MapToEntity(SalesInvoiceDto dto)
        {
            if (dto == null) return null;
            return new SalesInvoice
            {
                Id = dto.Id,
                // Map other properties here
            };
        }
    }
}
