using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gearbox.Domain.Entities
{
    public class Staff
    {
        [Key]
        public Guid UserId { get; set; }
      
        public string Department { get; set; }
        public string JobTitle { get; set; }
        public DateTime HireDate { get; set; } = DateTime.UtcNow;

        // Navigations
        public AppUser User { get; set; }
        public ICollection<SalesInvoice> SalesInvoices { get; set; } = new List<SalesInvoice>();
    }
}
