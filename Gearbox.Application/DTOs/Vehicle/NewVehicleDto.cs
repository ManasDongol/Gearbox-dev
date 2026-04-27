namespace Gearbox.Application.DTOs;

public class NewVehicleDto
{
    public Guid CustomerId { get; set; }
    public string NumberPlate { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public string VIN { get; set; }
    public DateTime CreatedAt { get; set; }
}