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
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CustomerDto>> GetAllAsync()
        {
            var entities = await _unitOfWork.Customers.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<CustomerDto> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.Customers.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<CustomerDto> AddAsync(CustomerDto dto)
        {
            var entity = MapToEntity(dto);
            entity.Id = Guid.NewGuid(); // ensuring a new ID
            await _unitOfWork.Customers.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, CustomerDto dto)
        {
            var entity = await _unitOfWork.Customers.GetByIdAsync(id);
            if (entity != null)
            {
                // Assign new values from dto
                // entity.SomeProperty = dto.SomeProperty;
                _unitOfWork.Customers.Update(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.Customers.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.Customers.Remove(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        // Manual Mapping Methods
        private CustomerDto MapToDto(Customer entity)
        {
            if (entity == null) return null;
            return new CustomerDto
            {
                Id = entity.Id,
                // Map other properties here
            };
        }

        private Customer MapToEntity(CustomerDto dto)
        {
            if (dto == null) return null;
            return new Customer
            {
                Id = dto.Id,
                // Map other properties here
            };
        }
    }
}
