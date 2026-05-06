using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Gearbox.Domain.Entities
{
    public class AppUser :IdentityUser<Guid>
    {
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        [StringLength(50)]   
        public string? Address { get; set; }
        [StringLength(50)]   
        public string FirstName { get; set; } = string.Empty;
        [StringLength(50)]     
        public string LastName { get; set; } = string.Empty;
        
        


        // Navigations
        public Customer Customer { get; set; }
        public Staff Staff { get; set; }
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
