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
        private readonly IUnitOfWork _unitOfWork;

        public PartRequestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PartRequestDto>> GetAllAsync()
        {
            var entities = await _unitOfWork.PartRequests.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<PartRequestDto> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.PartRequests.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<PartRequestDto> AddAsync(PartRequestDto dto)
        {
            var entity = MapToEntity(dto);
            entity.Id = Guid.NewGuid(); // ensuring a new ID
            await _unitOfWork.PartRequests.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, PartRequestDto dto)
        {
            var entity = await _unitOfWork.PartRequests.GetByIdAsync(id);
            if (entity != null)
            {
                // Assign new values from dto
                // entity.SomeProperty = dto.SomeProperty;
                _unitOfWork.PartRequests.Update(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.PartRequests.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.PartRequests.Remove(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        // Manual Mapping Methods
        private PartRequestDto MapToDto(PartRequest entity)
        {
            if (entity == null) return null;
            return new PartRequestDto
            {
                Id = entity.Id,
                // Map other properties here
            };
        }

        private PartRequest MapToEntity(PartRequestDto dto)
        {
            if (dto == null) return null;
            return new PartRequest
            {
                Id = dto.Id,
                // Map other properties here
            };
        }
    }
}
