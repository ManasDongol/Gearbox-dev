using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gearbox.Application.DTOs;

namespace Gearbox.Application.Interfaces
{
    public interface ISalesInvoiceItemService
    {
        Task<IEnumerable<SalesInvoiceItemDto>> GetAllAsync();
        Task<SalesInvoiceItemDto> GetByIdAsync(Guid id);
        Task<SalesInvoiceItemDto> AddAsync(SalesInvoiceItemDto dto);
        Task UpdateAsync(Guid id, SalesInvoiceItemDto dto);
        Task DeleteAsync(Guid id);
    }
}
