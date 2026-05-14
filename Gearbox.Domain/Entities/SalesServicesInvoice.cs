namespace Gearbox.Domain.Entities;

public class SalesServicesInvoice
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; }

    public Guid StaffId { get; set; }
    public Staff Staff { get; set; }

    public Guid? AppointmentId { get; set; } // optional
    public Appointment? Appointment { get; set; }

    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool PaymentStatus { get; set; } = false;

    public ICollection<SalesServicesInvoiceItem> Items { get; set; }
}