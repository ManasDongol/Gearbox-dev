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
        private readonly ICustomerRepository _repository;

        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CustomerDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<CustomerDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<CustomerDto> AddAsync(CustomerDto dto)
        {
            var entity = MapToEntity(dto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, CustomerDto dto)
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

        private CustomerDto MapToDto(Customer entity)
        {
            if (entity == null) return null;
            return new CustomerDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                FullName = entity.FullName,
                PhoneNumber = entity.Phone,
                Address = entity.Address,
                TotalSpent = entity.TotalSpent,
                PendingCredits = entity.PendingCredits,
                RegisteredSince = entity.RegisteredSince,
            };
        }

        private Customer MapToEntity(CustomerDto dto)
        {
            if (dto == null) return null;
            return new Customer
            {
                Id = dto.Id,
                UserId = dto.UserId,
                FullName = dto.FullName,
                Phone = dto.PhoneNumber,
                Address = dto.Address,
                TotalSpent = dto.TotalSpent,
                PendingCredits = dto.PendingCredits,
                RegisteredSince = dto.RegisteredSince,
            };
        }
    }
}
