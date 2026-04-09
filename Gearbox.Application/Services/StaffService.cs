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
        private readonly IStaffRepository _repository;

        public StaffService(IStaffRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<StaffDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<StaffDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<StaffDto> AddAsync(StaffDto dto)
        {
            var entity = MapToEntity(dto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, StaffDto dto)
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

        private StaffDto MapToDto(Staff entity)
        {
            if (entity == null) return null;
            return new StaffDto
            {
           
                UserId = entity.UserId,
                FullName = entity.FullName,
                Department = entity.Department,
                JobTitle = entity.JobTitle,
              
            };
        }

        private Staff MapToEntity(StaffDto dto)
        {
            if (dto == null) return null;
            return new Staff
            {
             
                UserId = dto.UserId,
                FullName = dto.FullName,
                Department = dto.Department,
                JobTitle = dto.JobTitle,
                HireDate = DateTime.UtcNow
            };
        }
    }
}
