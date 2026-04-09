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
        private readonly IServiceDetailsRepository _repository;

        public ServiceDetailsService(IServiceDetailsRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ServiceDetailsDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<ServiceDetailsDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<ServiceDetailsDto> AddAsync(ServiceDetailsDto dto)
        {
            var entity = MapToEntity(dto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, ServiceDetailsDto dto)
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

        private ServiceDetailsDto MapToDto(ServiceDetails entity)
        {
            if (entity == null) return null;
            return new ServiceDetailsDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                BasePrice = entity.BasePrice,
            };
        }

        private ServiceDetails MapToEntity(ServiceDetailsDto dto)
        {
            if (dto == null) return null;
            return new ServiceDetails
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                BasePrice = dto.BasePrice,
            };
        }
    }
}
