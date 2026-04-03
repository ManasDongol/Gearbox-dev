using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gearbox.Application.DTOs;

namespace Gearbox.Application.Interfaces
{
    public interface IServiceBillService
    {
        Task<IEnumerable<ServiceBillDto>> GetAllAsync();
        Task<ServiceBillDto> GetByIdAsync(Guid id);
        Task<ServiceBillDto> AddAsync(ServiceBillDto dto);
        Task UpdateAsync(Guid id, ServiceBillDto dto);
        Task DeleteAsync(Guid id);
    }
}
