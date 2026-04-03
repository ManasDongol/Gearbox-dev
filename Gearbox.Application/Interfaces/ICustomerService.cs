using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gearbox.Application.DTOs;

namespace Gearbox.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDto>> GetAllAsync();
        Task<CustomerDto> GetByIdAsync(Guid id);
        Task<CustomerDto> AddAsync(CustomerDto dto);
        Task UpdateAsync(Guid id, CustomerDto dto);
        Task DeleteAsync(Guid id);
    }
}
