using System;
using System.Collections.Generic;

namespace Gearbox.Domain.Entities
{
    public class PurchaseInvoice
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid VendorId { get; set; }
        
        public string InvoiceNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigations
        public Vendor Vendor { get; set; }
        public ICollection<PurchaseInvoiceItem> Items { get; set; } = new List<PurchaseInvoiceItem>();
    }
}
