using System;
using System.Collections.Generic;

namespace Gearbox.Domain.Entities
{
    public class Staff
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string FullName { get; set; } 
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string Department { get; set; }
        public string JobTitle { get; set; }
        public DateTime HireDate { get; set; } = DateTime.UtcNow;

        // Navigations
        public AppUser User { get; set; }
        public ICollection<SalesInvoice> SalesInvoices { get; set; } = new List<SalesInvoice>();
    }
}
