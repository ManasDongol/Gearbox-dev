using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gearbox.Application.DTOs;

namespace Gearbox.Application.Interfaces
{
    public interface IPartRequestService
    {
        Task<IEnumerable<PartRequestDto>> GetAllAsync();
        Task<PartRequestDto> GetByIdAsync(Guid id);
        Task<PartRequestDto> AddAsync(NewPartRequestDto dto);
        Task UpdateAsync(Guid id, PartRequestDto dto);
        Task DeleteAsync(Guid id);
    }
}
