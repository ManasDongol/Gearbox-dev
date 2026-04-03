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
    public class StaffService : IStaffService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StaffService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<StaffDto>> GetAllAsync()
        {
            var entities = await _unitOfWork.Staffs.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<StaffDto> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.Staffs.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<StaffDto> AddAsync(StaffDto dto)
        {
            var entity = MapToEntity(dto);
            entity.Id = Guid.NewGuid(); // ensuring a new ID
            await _unitOfWork.Staffs.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, StaffDto dto)
        {
            var entity = await _unitOfWork.Staffs.GetByIdAsync(id);
            if (entity != null)
            {
                // Assign new values from dto
                // entity.SomeProperty = dto.SomeProperty;
                _unitOfWork.Staffs.Update(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.Staffs.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.Staffs.Remove(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        // Manual Mapping Methods
        private StaffDto MapToDto(Staff entity)
        {
            if (entity == null) return null;
            return new StaffDto
            {
                Id = entity.Id,
                // Map other properties here
            };
        }

        private Staff MapToEntity(StaffDto dto)
        {
            if (dto == null) return null;
            return new Staff
            {
                Id = dto.Id,
                // Map other properties here
            };
        }
    }
}
