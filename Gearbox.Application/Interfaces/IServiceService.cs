using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gearbox.Application.DTOs;

namespace Gearbox.Application.Interfaces
{
    public interface IServiceService
    {
        Task<IEnumerable<ServiceDto>> GetAllAsync();
        Task<ServiceDto?> GetByIdAsync(Guid id);
        Task<ServiceDto> AddAsync(NewServiceDto dto);
        Task UpdateAsync(Guid id, ServiceDto dto);
        Task DeleteAsync(Guid id);
    }
}
