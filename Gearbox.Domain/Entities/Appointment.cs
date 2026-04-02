using System;
using System.Collections.Generic;

namespace Gearbox.Domain.Entities
{
    public class Appointment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CustomerId { get; set; }
        public Guid VehicleId { get; set; }
        
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; } // e.g., "Pending", "Confirmed", "Completed", "Cancelled"
        public string Notes { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigations
        public Customer Customer { get; set; }
        public Vehicle Vehicle { get; set; }
        public ICollection<ServiceDetails> Services { get; set; } = new List<ServiceDetails>();
    }
}
