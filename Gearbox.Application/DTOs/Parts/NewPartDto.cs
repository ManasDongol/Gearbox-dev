namespace Gearbox.Application.DTOs;

public class NewPartDto
{


    public string Name { get; set; }
    public string Description { get; set; }
    public string PartNumber { get; set; }
    public decimal SellingPrice { get; set; }
    public int StockQuantity { get; set; }
    public Guid VendorId { get; set; }
}