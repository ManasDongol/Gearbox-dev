using System;
using System.Collections.Generic;

namespace Gearbox.Domain.Entities
{
    public class SalesInvoice
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CustomerId { get; set; }
        public Guid StaffId { get; set; }
        
        public string InvoiceNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; } // e.g., 10% loyalty discount
        public bool IsLoyaltyDiscountApplied { get; set; }
        
        public bool IsPaid { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigations
        public Customer Customer { get; set; }
        public Staff Staff { get; set; }
        public ICollection<SalesInvoiceItem> Items { get; set; } = new List<SalesInvoiceItem>();
    }
}
