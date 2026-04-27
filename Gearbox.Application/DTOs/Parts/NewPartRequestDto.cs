namespace Gearbox.Application.DTOs;

public class NewPartRequestDto
{
    public Guid CustomerId { get; set; }
    public string PartName { get; set; }
    public string Description { get; set; }
    public bool IsFulfilled { get; set; }
    public DateTime RequestDate { get; set; }
}