using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gearbox.Application.DTOs;

namespace Gearbox.Application.Interfaces
{
    public interface ISalesServicesInvoiceService
    {
        Task<IEnumerable<SalesServicesInvoiceDto>> GetAllAsync();
        Task<SalesServicesInvoiceDto> GetByIdAsync(Guid id);
        Task<SalesServicesInvoiceDto> AddAsync(NewSalesServicesInvoiceDto dto);
        Task UpdateAsync(Guid id, SalesServicesInvoiceDto dto);
        Task DeleteAsync(Guid id);
    }

    public interface ISalesServicesInvoiceItemService
    {
        Task<IEnumerable<SalesServicesInvoiceItemDto>> GetAllAsync();
        Task<SalesServicesInvoiceItemDto> GetByIdAsync(Guid id);
        Task<SalesServicesInvoiceItemDto> AddAsync(SalesServicesInvoiceItemDto dto);
        Task UpdateAsync(Guid id, SalesServicesInvoiceItemDto dto);
        Task DeleteAsync(Guid id);
    }
}
