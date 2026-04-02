using System;

namespace Gearbox.Domain.Entities
{
    public class PurchaseInvoiceItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PurchaseInvoiceId { get; set; }
        public Guid PartId { get; set; }
        
        public int Quantity { get; set; }
        public decimal CostPrice { get; set; }
        public decimal TotalCost => Quantity * CostPrice;

        // Navigations
        public PurchaseInvoice PurchaseInvoice { get; set; }
        public Part Part { get; set; }
    }
}
