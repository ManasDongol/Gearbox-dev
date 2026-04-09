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
        private readonly IVehicleRepository _repository;

        public VehicleService(IVehicleRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<VehicleDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<VehicleDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<VehicleDto> AddAsync(VehicleDto dto)
        {
            var entity = MapToEntity(dto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, VehicleDto dto)
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

        private VehicleDto MapToDto(Vehicle entity)
        {
            if (entity == null) return null;
            return new VehicleDto
            {
                Id = entity.Id,
                CustomerId = entity.CustomerId,
                NumberPlate = entity.NumberPlate,
                Make = entity.Make,
                Model = entity.Model,
                Year = entity.Year,
                VIN = entity.VIN,
                CreatedAt = entity.CreatedAt,
            };
        }

        private Vehicle MapToEntity(VehicleDto dto)
        {
            if (dto == null) return null;
            return new Vehicle
            {
                Id = dto.Id,
                CustomerId = dto.CustomerId,
                NumberPlate = dto.NumberPlate,
                Make = dto.Make,
                Model = dto.Model,
                Year = dto.Year,
                VIN = dto.VIN,
                CreatedAt = dto.CreatedAt,
            };
        }
    }
}
