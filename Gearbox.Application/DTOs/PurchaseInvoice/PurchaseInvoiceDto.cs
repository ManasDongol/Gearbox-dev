using System;

namespace Gearbox.Application.DTOs
{
    public class PurchaseInvoiceDto
    {
        public Guid Id { get; set; }
        public Guid VendorId { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
