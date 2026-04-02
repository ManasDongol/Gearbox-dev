using System;
using System.Collections.Generic;

namespace Gearbox.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } // e.g., "Admin", "Staff", "Customer"
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public bool emailVerified { get; set; } = false;

        // Navigations
        public Customer Customer { get; set; }
        public Staff Staff { get; set; }
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
