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
        private readonly IUnitOfWork _unitOfWork;

        public ServiceHistoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ServiceHistoryDto>> GetAllAsync()
        {
            var entities = await _unitOfWork.ServiceHistories.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<ServiceHistoryDto> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.ServiceHistories.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<ServiceHistoryDto> AddAsync(ServiceHistoryDto dto)
        {
            var entity = MapToEntity(dto);
            entity.Id = Guid.NewGuid(); // ensuring a new ID
            await _unitOfWork.ServiceHistories.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, ServiceHistoryDto dto)
        {
            var entity = await _unitOfWork.ServiceHistories.GetByIdAsync(id);
            if (entity != null)
            {
                // Assign new values from dto
                // entity.SomeProperty = dto.SomeProperty;
                _unitOfWork.ServiceHistories.Update(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.ServiceHistories.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.ServiceHistories.Remove(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        // Manual Mapping Methods
        private ServiceHistoryDto MapToDto(ServiceHistory entity)
        {
            if (entity == null) return null;
            return new ServiceHistoryDto
            {
                Id = entity.Id,
                // Map other properties here
            };
        }

        private ServiceHistory MapToEntity(ServiceHistoryDto dto)
        {
            if (dto == null) return null;
            return new ServiceHistory
            {
                Id = dto.Id,
                // Map other properties here
            };
        }
    }
}
