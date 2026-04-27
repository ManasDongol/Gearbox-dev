using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gearbox.Application.DTOs;
using Gearbox.Application.DTOs.Customer;

namespace Gearbox.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDto>> GetAllAsync();
        Task<CustomerDto> GetByIdAsync(Guid id);
        Task<NewCustomerDto> AddAsync(NewCustomerDto dto);
        Task UpdateAsync(Guid id, CustomerDto dto);
        Task DeleteAsync(Guid id);
    }
}
