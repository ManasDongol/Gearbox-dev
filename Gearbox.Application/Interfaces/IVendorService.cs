using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gearbox.Application.DTOs;
using Gearbox.Application.DTOs.Vendor;

namespace Gearbox.Application.Interfaces
{
    public interface IVendorService
    {
        Task<IEnumerable<VendorDto>> GetAllAsync();
        Task<VendorDto> GetByIdAsync(Guid id);
        Task<VendorDto> AddAsync(NewVendorDto dto);
        Task UpdateAsync(Guid id, VendorDto dto);
        Task DeleteAsync(Guid id);
    }
}
