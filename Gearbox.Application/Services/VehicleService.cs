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
    public class VehicleService : IVehicleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VehicleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<VehicleDto>> GetAllAsync()
        {
            var entities = await _unitOfWork.Vehicles.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<VehicleDto> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.Vehicles.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<VehicleDto> AddAsync(VehicleDto dto)
        {
            var entity = MapToEntity(dto);
            entity.Id = Guid.NewGuid(); // ensuring a new ID
            await _unitOfWork.Vehicles.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, VehicleDto dto)
        {
            var entity = await _unitOfWork.Vehicles.GetByIdAsync(id);
            if (entity != null)
            {
                // Assign new values from dto
                // entity.SomeProperty = dto.SomeProperty;
                _unitOfWork.Vehicles.Update(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.Vehicles.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.Vehicles.Remove(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        // Manual Mapping Methods
        private VehicleDto MapToDto(Vehicle entity)
        {
            if (entity == null) return null;
            return new VehicleDto
            {
                Id = entity.Id,
                // Map other properties here
            };
        }

        private Vehicle MapToEntity(VehicleDto dto)
        {
            if (dto == null) return null;
            return new Vehicle
            {
                Id = dto.Id,
                // Map other properties here
            };
        }
    }
}
