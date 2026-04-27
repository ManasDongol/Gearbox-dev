namespace Gearbox.Application.DTOs;

public class NewPurchaseInvoiceDto
{
    public Guid VendorId { get; set; }
    public string InvoiceNumber { get; set; }
    public decimal TotalAmount { get; set; }
    public List<PurchaseInvoiceItemDto> Items { get; set; } = new List<PurchaseInvoiceItemDto>();
}