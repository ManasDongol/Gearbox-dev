using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gearbox.Application.DTOs;

namespace Gearbox.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto> GetByIdAsync(Guid id);
        Task<UserDto> AddAsync(UserDto dto);
        Task UpdateAsync(string id, UserDto dto);
        Task DeleteAsync(Guid id);
        
        
        
    }
}
