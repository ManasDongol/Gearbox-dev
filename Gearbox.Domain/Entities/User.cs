using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Gearbox.Domain.Entities
{
    public class AppUser :IdentityUser<Guid>
    {
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        
   

        // Navigations
        public Customer Customer { get; set; }
        public Staff Staff { get; set; }
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
