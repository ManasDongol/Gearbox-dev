using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gearbox.Application.DTOs;

namespace Gearbox.Application.Interfaces
{
    public interface IPartService
    {
        Task<IEnumerable<PartDto>> GetAllAsync();
        Task<PartDto> GetByIdAsync(Guid id);
        Task<PartDto> AddAsync(NewPartDto dto);
        Task UpdateAsync(Guid id, PartDto dto);
        Task DeleteAsync(Guid id);
    }
}
