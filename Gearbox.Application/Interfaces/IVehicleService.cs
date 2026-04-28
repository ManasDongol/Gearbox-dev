using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gearbox.Application.DTOs;

namespace Gearbox.Application.Interfaces
{
    public interface IVehicleService
    {
        Task<IEnumerable<VehicleDto>> GetAllAsync();
        Task<VehicleDto> GetByIdAsync(Guid id);
        Task<VehicleDto> AddAsync(NewVehicleDto dto);
        Task UpdateAsync(Guid id, VehicleDto dto);
        Task DeleteAsync(Guid id);
    }
}
