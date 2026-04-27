namespace Gearbox.Application.DTOs;

public class NewPurchaseInvoiceItemDto
{
    
    public Guid PurchaseInvoiceId { get; set; }
    public List<PurchaseInvoiceItemDto> _items { get; set; }
}