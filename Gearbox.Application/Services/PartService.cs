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
    public class PartService : IPartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PartDto>> GetAllAsync()
        {
            var entities = await _unitOfWork.Parts.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<PartDto> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.Parts.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<PartDto> AddAsync(PartDto dto)
        {
            var entity = MapToEntity(dto);
            entity.Id = Guid.NewGuid(); // ensuring a new ID
            await _unitOfWork.Parts.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, PartDto dto)
        {
            var entity = await _unitOfWork.Parts.GetByIdAsync(id);
            if (entity != null)
            {
                // Assign new values from dto
                // entity.SomeProperty = dto.SomeProperty;
                _unitOfWork.Parts.Update(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.Parts.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.Parts.Remove(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        // Manual Mapping Methods
        private PartDto MapToDto(Part entity)
        {
            if (entity == null) return null;
            return new PartDto
            {
                Id = entity.Id,
                // Map other properties here
            };
        }

        private Part MapToEntity(PartDto dto)
        {
            if (dto == null) return null;
            return new Part
            {
                Id = dto.Id,
                // Map other properties here
            };
        }
    }
}
