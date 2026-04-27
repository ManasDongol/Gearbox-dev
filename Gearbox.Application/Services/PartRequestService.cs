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
    public class PartRequestService : IPartRequestService
    {
        private readonly IPartRequestRepository _repository;

        public PartRequestService(IPartRequestRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PartRequestDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<PartRequestDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<PartRequestDto> AddAsync(NewPartRequestDto dto)
        {
            var entity = MapToEntity(dto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, PartRequestDto dto)
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

        private PartRequestDto MapToDto(PartRequest entity)
        {
            if (entity == null) return null;
            return new PartRequestDto
            {
                Id = entity.Id,
                CustomerId = entity.CustomerId,
                PartName = entity.PartName,
                Description = entity.Description,
                IsFulfilled = entity.IsFulfilled,
                RequestDate = entity.RequestDate,
            };
        }

        private PartRequest MapToEntity(PartRequestDto dto)
        {
            if (dto == null) return null;
            return new PartRequest
            {
                Id = dto.Id,
                CustomerId = dto.CustomerId,
                PartName = dto.PartName,
                Description = dto.Description,
                IsFulfilled = dto.IsFulfilled,
                RequestDate = dto.RequestDate,
            };
        }
        private PartRequest MapToEntity(NewPartRequestDto dto)
        {
            if (dto == null) return null;
            return new PartRequest
            {
                CustomerId = dto.CustomerId,
                PartName = dto.PartName,
                Description = dto.Description,
                IsFulfilled = dto.IsFulfilled,
                RequestDate = dto.RequestDate,
            };
        }
        
    
        
    }
}
