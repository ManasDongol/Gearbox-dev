using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gearbox.Application.DTOs;

namespace Gearbox.Application.Interfaces
{
    public interface IPurchaseInvoiceItemService
    {
        Task<IEnumerable<PurchaseInvoiceItemDto>> GetAllAsync();
        Task<PurchaseInvoiceItemDto> GetByIdAsync(Guid id);
        Task<NewPurchaseInvoiceItemDto> AddAsync(NewPurchaseInvoiceItemDto dto);
        Task UpdateAsync(Guid id, PurchaseInvoiceItemDto dto);
        Task DeleteAsync(Guid id);
    }
}
