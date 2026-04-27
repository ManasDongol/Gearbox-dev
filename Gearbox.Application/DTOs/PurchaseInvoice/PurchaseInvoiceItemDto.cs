using System;

namespace Gearbox.Application.DTOs
{
    public class PurchaseInvoiceItemDto
    {
        public Guid PartId { get; set; }
        public int Quantity { get; set; }
        public decimal CostPrice { get; set; }
    }
}
