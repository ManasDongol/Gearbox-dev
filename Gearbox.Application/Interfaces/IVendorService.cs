using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gearbox.Application.DTOs;

namespace Gearbox.Application.Interfaces
{
    public interface IVendorService
    {
        Task<IEnumerable<VendorDto>> GetAllAsync();
        Task<VendorDto> GetByIdAsync(Guid id);
        Task<VendorDto> AddAsync(VendorDto dto);
        Task UpdateAsync(Guid id, VendorDto dto);
        Task DeleteAsync(Guid id);
    }
}
