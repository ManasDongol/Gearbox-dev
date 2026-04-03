using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gearbox.Application.DTOs;

namespace Gearbox.Application.Interfaces
{
    public interface IServiceDetailsService
    {
        Task<IEnumerable<ServiceDetailsDto>> GetAllAsync();
        Task<ServiceDetailsDto> GetByIdAsync(Guid id);
        Task<ServiceDetailsDto> AddAsync(ServiceDetailsDto dto);
        Task UpdateAsync(Guid id, ServiceDetailsDto dto);
        Task DeleteAsync(Guid id);
    }
}
