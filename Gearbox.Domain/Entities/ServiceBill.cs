using System;

namespace Gearbox.Domain.Entities
{
    public class ServiceBill
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        // Optional Link to an Appointment / ServiceHistory if needed
        public Guid? AppointmentId { get; set; }
        public Guid? ServiceHistoryId { get; set; }

        public decimal TotalAmount { get; set; }
        public bool IsPaid { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigations
        public Appointment Appointment { get; set; }
        public ServiceHistory ServiceHistory { get; set; }
    }
}
