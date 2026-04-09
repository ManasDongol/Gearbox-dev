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
        private readonly IServiceBillRepository _repository;

        public ServiceBillService(IServiceBillRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ServiceBillDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<ServiceBillDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<ServiceBillDto> AddAsync(ServiceBillDto dto)
        {
            var entity = MapToEntity(dto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, ServiceBillDto dto)
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

        private ServiceBillDto MapToDto(ServiceBill entity)
        {
            if (entity == null) return null;
            return new ServiceBillDto
            {
                Id = entity.Id,
                AppointmentId = entity.AppointmentId,
                ServiceHistoryId = entity.ServiceHistoryId,
                TotalAmount = entity.TotalAmount,
                IsPaid = entity.IsPaid,
                CreatedDate = entity.CreatedDate,
              
            };
        }

        private ServiceBill MapToEntity(ServiceBillDto dto)
        {
            if (dto == null) return null;
            return new ServiceBill
            {
                Id = dto.Id,
                AppointmentId = dto.AppointmentId,
                ServiceHistoryId = dto.ServiceHistoryId,
                TotalAmount = dto.TotalAmount,
                IsPaid = dto.IsPaid,
                CreatedDate = dto.CreatedDate,
     
            };
        }
    }
}
