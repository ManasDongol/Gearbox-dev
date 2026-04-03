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
    public class ServiceDetailsService : IServiceDetailsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServiceDetailsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ServiceDetailsDto>> GetAllAsync()
        {
            var entities = await _unitOfWork.ServiceDetails.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<ServiceDetailsDto> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.ServiceDetails.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<ServiceDetailsDto> AddAsync(ServiceDetailsDto dto)
        {
            var entity = MapToEntity(dto);
            entity.Id = Guid.NewGuid(); // ensuring a new ID
            await _unitOfWork.ServiceDetails.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, ServiceDetailsDto dto)
        {
            var entity = await _unitOfWork.ServiceDetails.GetByIdAsync(id);
            if (entity != null)
            {
                // Assign new values from dto
                // entity.SomeProperty = dto.SomeProperty;
                _unitOfWork.ServiceDetails.Update(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.ServiceDetails.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.ServiceDetails.Remove(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        // Manual Mapping Methods
        private ServiceDetailsDto MapToDto(ServiceDetails entity)
        {
            if (entity == null) return null;
            return new ServiceDetailsDto
            {
                Id = entity.Id,
                // Map other properties here
            };
        }

        private ServiceDetails MapToEntity(ServiceDetailsDto dto)
        {
            if (dto == null) return null;
            return new ServiceDetails
            {
                Id = dto.Id,
                // Map other properties here
            };
        }
    }
}
