using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gearbox.Application.DTOs;

namespace Gearbox.Application.Interfaces
{
    public interface IServiceHistoryService
    {
        Task<IEnumerable<ServiceHistoryDto>> GetAllAsync();
        Task<ServiceHistoryDto> GetByIdAsync(Guid id);
        Task<ServiceHistoryDto> AddAsync(ServiceHistoryDto dto);
        Task UpdateAsync(Guid id, ServiceHistoryDto dto);
        Task DeleteAsync(Guid id);
    }
}
