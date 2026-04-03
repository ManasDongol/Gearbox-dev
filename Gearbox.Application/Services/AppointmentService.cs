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
        private readonly IUnitOfWork _unitOfWork;

        public AppointmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<AppointmentDto>> GetAllAsync()
        {
            var entities = await _unitOfWork.Appointments.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<AppointmentDto> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<AppointmentDto> AddAsync(AppointmentDto dto)
        {
            var entity = MapToEntity(dto);
            entity.Id = Guid.NewGuid(); // ensuring a new ID
            await _unitOfWork.Appointments.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, AppointmentDto dto)
        {
            var entity = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (entity != null)
            {
                // Assign new values from dto
                // entity.SomeProperty = dto.SomeProperty;
                _unitOfWork.Appointments.Update(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.Appointments.Remove(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        // Manual Mapping Methods
        private AppointmentDto MapToDto(Appointment entity)
        {
            if (entity == null) return null;
            return new AppointmentDto
            {
                Id = entity.Id,
                // Map other properties here
            };
        }

        private Appointment MapToEntity(AppointmentDto dto)
        {
            if (dto == null) return null;
            return new Appointment
            {
                Id = dto.Id,
                // Map other properties here
            };
        }
    }
}
