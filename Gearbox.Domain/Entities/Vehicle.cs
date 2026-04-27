using System;
using System.Collections.Generic;
using Gearbox.Domain.ENUMs;

namespace Gearbox.Domain.Entities
{
    public class Vehicle
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CustomerId { get; set; }
        public string NumberPlate { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string VIN { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public VehicleType VehicleType { get; set; }

        // Navigations
        public Customer Customer { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<ServiceHistory> ServiceHistories { get; set; } = new List<ServiceHistory>();
    }
}
