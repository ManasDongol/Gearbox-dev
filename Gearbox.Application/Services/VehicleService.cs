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

        public async Task<IEnumerable<VehicleDto>> GetAllByVehicleIdAsync(Guid userId)
        {
            var entities = await _repository.GetCustomerVehicles(userId);
            return entities.Select(e => MapToDto(e));
        }

        public async Task<VehicleDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<VehicleDto> AddAsync(NewVehicleDto dto)
        {
            var entity = MapToEntity(dto);
            entity.CreatedAt = DateTime.UtcNow;
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, VehicleDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                entity.VehicleType = dto.VehicleType;
                entity.VIN = dto.VIN;
                entity.Make = dto.Make;
                entity.Model = dto.Model;
                entity.Year = dto.Year;
                entity.NumberPlate = dto.NumberPlate;
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
                VehicleType = entity.VehicleType,
                
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
                VehicleType = dto.VehicleType,
               
            };
        }
        
        private Vehicle MapToEntity(NewVehicleDto dto)
        {
            if (dto == null) return null;
            return new Vehicle
            {
              
                CustomerId = dto.CustomerId,
                NumberPlate = dto.NumberPlate,
                Make = dto.Make,
                Model = dto.Model,
                Year = dto.Year,
                VIN = dto.VIN,
                VehicleType = dto.VehicleType,
                CreatedAt = DateTime.UtcNow,
            };
        }
    }
}
