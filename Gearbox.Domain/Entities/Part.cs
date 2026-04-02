using System;

namespace Gearbox.Domain.Entities
{
    public class Part
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public string PartNumber { get; set; }
        public decimal SellingPrice { get; set; }
        public int StockQuantity { get; set; }
        
        // Relationship to vendor
        public Guid VendorId { get; set; }
        
        // Navigations
        public Vendor Vendor { get; set; }
    }
}
