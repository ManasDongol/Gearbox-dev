using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gearbox.Application.DTOs;
using Gearbox.Application.DTOs.Staff;

namespace Gearbox.Application.Interfaces
{
    public interface IStaffService
    {
        Task<IEnumerable<StaffDto>> GetAllAsync();
        Task<StaffDto> GetByIdAsync(Guid id);
        Task<StaffDto> AddAsync(NewStaffDto dto);
        Task UpdateAsync(Guid id, StaffDto dto);
        Task DeleteAsync(Guid id);
    }
}
