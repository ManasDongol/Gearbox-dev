namespace Gearbox.Application.DTOs;

public class NewAppointmentDto
{
    public Guid CustomerId { get; set; }
    public Guid VehicleId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; }
    public string Notes { get; set; }
    public DateTime CreatedDate { get; set; }
}