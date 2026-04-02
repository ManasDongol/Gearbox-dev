using System;

namespace Gearbox.Domain.Entities
{
    public class PartRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CustomerId { get; set; }
        public string PartName { get; set; }
        public string Description { get; set; }
        public bool IsFulfilled { get; set; } = false;
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        // Navigations
        public Customer Customer { get; set; }
    }
}
