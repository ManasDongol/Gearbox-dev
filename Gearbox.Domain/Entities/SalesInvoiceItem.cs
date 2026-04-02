using System;

namespace Gearbox.Domain.Entities
{
    public class SalesInvoiceItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SalesInvoiceId { get; set; }
        public Guid PartId { get; set; }
        
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;

        // Navigations
        public SalesInvoice SalesInvoice { get; set; }
        public Part Part { get; set; }
    }
}
