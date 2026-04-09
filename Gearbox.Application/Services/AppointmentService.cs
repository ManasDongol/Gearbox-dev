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
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repository;

        public AppointmentService(IAppointmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<AppointmentDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<AppointmentDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<AppointmentDto> AddAsync(AppointmentDto dto)
        {
            var entity = MapToEntity(dto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, AppointmentDto dto)
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

        private AppointmentDto MapToDto(Appointment entity)
        {
            if (entity == null) return null;
            return new AppointmentDto
            {
                Id = entity.Id,
                CustomerId = entity.CustomerId,
                VehicleId = entity.VehicleId,
                AppointmentDate = entity.AppointmentDate,
                Status = entity.Status,
                Notes = entity.Notes,
                CreatedDate = entity.CreatedDate,
            };
        }

        private Appointment MapToEntity(AppointmentDto dto)
        {
            if (dto == null) return null;
            return new Appointment
            {
                Id = dto.Id,
                CustomerId = dto.CustomerId,
                VehicleId = dto.VehicleId,
                AppointmentDate = dto.AppointmentDate,
                Status = dto.Status,
                Notes = dto.Notes,
                CreatedDate = dto.CreatedDate,
            };
        }
    }
}
