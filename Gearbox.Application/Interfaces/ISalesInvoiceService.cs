using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gearbox.Application.DTOs;

namespace Gearbox.Application.Interfaces
{
    public interface ISalesInvoiceService
    {
        Task<IEnumerable<SalesInvoiceDto>> GetAllAsync();
        Task<SalesInvoiceDto> GetByIdAsync(Guid id);
        Task<SalesInvoiceDto> AddAsync(SalesInvoiceDto dto);
        Task UpdateAsync(Guid id, SalesInvoiceDto dto);
        Task DeleteAsync(Guid id);
    }
}
