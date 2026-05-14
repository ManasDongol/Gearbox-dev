public class NewSalesServicesInvoiceDto
{
    public Guid CustomerId { get; set; }
    public Guid StaffId { get; set; }
    public Guid? AppointmentId { get; set; }
    public decimal DiscountAmount { get; set; }

    public List<NewSalesServicesInvoiceItemDto> Items { get; set; } = new();
}

public class NewSalesServicesInvoiceItemDto
{
    public Guid? PartId { get; set; }
    public Guid? ServiceId { get; set; }
    public Guid? VehicleId { get; set; }
    public string Type { get; set; } // "Part" or "Service"
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
