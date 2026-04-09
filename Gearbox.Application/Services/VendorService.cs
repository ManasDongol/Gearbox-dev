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
    public class VendorService : IVendorService
    {
        private readonly IVendorRepository _repository;

        public VendorService(IVendorRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<VendorDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<VendorDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<VendorDto> AddAsync(VendorDto dto)
        {
            var entity = MapToEntity(dto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, VendorDto dto)
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

        private VendorDto MapToDto(Vendor entity)
        {
            if (entity == null) return null;
            return new VendorDto
            {
                Id = entity.Id,
                Name = entity.Name,
                ContactPerson = entity.ContactPerson,
                PhoneNumber = entity.PhoneNumber,
                Email = entity.Email,
                Address = entity.Address,
            };
        }

        private Vendor MapToEntity(VendorDto dto)
        {
            if (dto == null) return null;
            return new Vendor
            {
                Id = dto.Id,
                Name = dto.Name,
                ContactPerson = dto.ContactPerson,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                Address = dto.Address,
            };
        }
    }
}
