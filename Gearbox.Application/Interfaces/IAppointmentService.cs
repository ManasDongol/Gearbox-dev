using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gearbox.Application.DTOs;

namespace Gearbox.Application.Interfaces
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentDto>> GetAllAsync();
        Task<AppointmentDto> GetByIdAsync(Guid id);
        Task<AppointmentDto> AddAsync(AppointmentDto dto);
        Task UpdateAsync(Guid id, AppointmentDto dto);
        Task DeleteAsync(Guid id);
    }
}
