using System;

namespace Gearbox.Domain.Entities
{
    public class ServiceReview
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CustomerId { get; set; }
        public Guid? AppointmentId { get; set; }
        
        public int Rating { get; set; } // e.g., 1 to 5
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

        // Navigations
        public Customer Customer { get; set; }
        public Appointment Appointment { get; set; }
    }
}
