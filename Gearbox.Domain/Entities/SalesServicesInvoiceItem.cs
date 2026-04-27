namespace Gearbox.Domain.Entities;

public class SalesServicesInvoiceItem
{
    public Guid Id { get; set; }

    public Guid SalesServicesInvoiceId { get; set; }
    public SalesServicesInvoice SalesServicesInvoice { get; set; }
    
    public Guid? PartId { get; set; }
    public Part? Part { get; set; }

    public Guid? ServiceDetailsId { get; set; }
    public ServiceDetails? ServiceDetails { get; set; }

    public string Type { get; set; } // "Part" or "Service"

    public int Quantity { get; set; } // for parts (can be 1 for service)
    public decimal UnitPrice { get; set; }
}