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
        private readonly IPartRepository _repository;

        public PartService(IPartRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PartDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<PartDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<PartDto> AddAsync(PartDto dto)
        {
            var entity = MapToEntity(dto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, PartDto dto)
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

        private PartDto MapToDto(Part entity)
        {
            if (entity == null) return null;
            return new PartDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                PartNumber = entity.PartNumber,
                SellingPrice = entity.SellingPrice,
                StockQuantity = entity.StockQuantity,
                VendorId = entity.VendorId,
            };
        }

        private Part MapToEntity(PartDto dto)
        {
            if (dto == null) return null;
            return new Part
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                PartNumber = dto.PartNumber,
                SellingPrice = dto.SellingPrice,
                StockQuantity = dto.StockQuantity,
                VendorId = dto.VendorId,
            };
        }
    }
}
