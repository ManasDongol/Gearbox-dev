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
        private readonly IUnitOfWork _unitOfWork;

        public VendorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<VendorDto>> GetAllAsync()
        {
            var entities = await _unitOfWork.Vendors.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<VendorDto> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.Vendors.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<VendorDto> AddAsync(VendorDto dto)
        {
            var entity = MapToEntity(dto);
            entity.Id = Guid.NewGuid(); // ensuring a new ID
            await _unitOfWork.Vendors.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, VendorDto dto)
        {
            var entity = await _unitOfWork.Vendors.GetByIdAsync(id);
            if (entity != null)
            {
                // Assign new values from dto
                // entity.SomeProperty = dto.SomeProperty;
                _unitOfWork.Vendors.Update(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.Vendors.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.Vendors.Remove(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        // Manual Mapping Methods
        private VendorDto MapToDto(Vendor entity)
        {
            if (entity == null) return null;
            return new VendorDto
            {
                Id = entity.Id,
                // Map other properties here
            };
        }

        private Vendor MapToEntity(VendorDto dto)
        {
            if (dto == null) return null;
            return new Vendor
            {
                Id = dto.Id,
                // Map other properties here
            };
        }
    }
}
