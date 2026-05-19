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
    public class ServiceHistoryService : IServiceHistoryService
    {
        private readonly IServiceHistoryRepository _repository;

        public ServiceHistoryService(IServiceHistoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ServiceHistoryDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<ServiceHistoryDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<ServiceHistoryDto> AddAsync(ServiceHistoryDto dto)
        {
            var entity = MapToEntity(dto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, ServiceHistoryDto dto)
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

        private ServiceHistoryDto MapToDto(ServiceHistory entity)
        {
            if (entity == null) return null;
            return new ServiceHistoryDto
            {
                Id = entity.Id,
                CustomerId = entity.CustomerId,
                VehicleId = entity.VehicleId,
                ServiceId = entity.ServiceId,
                ServiceDate = entity.ServiceDate,
                Notes = entity.Notes,
                TotalCost = entity.TotalCost,
            };
        }

        private ServiceHistory MapToEntity(ServiceHistoryDto dto)
        {
            if (dto == null) return null;
            return new ServiceHistory
            {
                Id = dto.Id,
                CustomerId = dto.CustomerId,
                VehicleId = dto.VehicleId,
                ServiceId = dto.ServiceId,
                ServiceDate = dto.ServiceDate,
                Notes = dto.Notes,
                TotalCost = dto.TotalCost,
            };
        }
    }
}
