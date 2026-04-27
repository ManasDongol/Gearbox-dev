using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gearbox.Application.DTOs;

namespace Gearbox.Application.Interfaces
{
    public interface IPurchaseInvoiceService
    {
        Task<IEnumerable<PurchaseInvoiceDto>> GetAllAsync();
        Task<PurchaseInvoiceDto> GetByIdAsync(Guid id);
        Task<PurchaseInvoiceDto> AddAsync(NewPurchaseInvoiceDto dto);
        Task UpdateAsync(Guid id, PurchaseInvoiceDto dto);
        Task DeleteAsync(Guid id);
    }
}
