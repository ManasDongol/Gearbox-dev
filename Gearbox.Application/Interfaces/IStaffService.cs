using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gearbox.Application.DTOs;

namespace Gearbox.Application.Interfaces
{
    public interface IStaffService
    {
        Task<IEnumerable<StaffDto>> GetAllAsync();
        Task<StaffDto> GetByIdAsync(Guid id);
        Task<StaffDto> AddAsync(StaffDto dto);
        Task UpdateAsync(Guid id, StaffDto dto);
        Task DeleteAsync(Guid id);
    }
}
