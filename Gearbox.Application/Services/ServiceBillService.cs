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
    public class ServiceBillService : IServiceBillService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServiceBillService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ServiceBillDto>> GetAllAsync()
        {
            var entities = await _unitOfWork.ServiceBills.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<ServiceBillDto> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.ServiceBills.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<ServiceBillDto> AddAsync(ServiceBillDto dto)
        {
            var entity = MapToEntity(dto);
            entity.Id = Guid.NewGuid(); // ensuring a new ID
            await _unitOfWork.ServiceBills.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, ServiceBillDto dto)
        {
            var entity = await _unitOfWork.ServiceBills.GetByIdAsync(id);
            if (entity != null)
            {
                // Assign new values from dto
                // entity.SomeProperty = dto.SomeProperty;
                _unitOfWork.ServiceBills.Update(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.ServiceBills.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.ServiceBills.Remove(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        // Manual Mapping Methods
        private ServiceBillDto MapToDto(ServiceBill entity)
        {
            if (entity == null) return null;
            return new ServiceBillDto
            {
                Id = entity.Id,
                // Map other properties here
            };
        }

        private ServiceBill MapToEntity(ServiceBillDto dto)
        {
            if (dto == null) return null;
            return new ServiceBill
            {
                Id = dto.Id,
                // Map other properties here
            };
        }
    }
}
