using System;
using System.Collections.Generic;

namespace Gearbox.Domain.Entities
{
    public class ServiceHistory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CustomerId { get; set; }
        public Guid VehicleId { get; set; }
        public Guid ServiceId { get; set; }
        
        public DateTime ServiceDate { get; set; }
        public string Notes { get; set; }
        public decimal TotalCost { get; set; }

        // Navigations
        public Customer Customer { get; set; }
        public Vehicle Vehicle { get; set; }
        public Service Service { get; set; }

    }
}
