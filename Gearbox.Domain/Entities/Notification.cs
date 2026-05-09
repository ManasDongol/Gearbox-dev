using System;

namespace Gearbox.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? UserId { get; set; }
        public string Message { get; set; } = null!;
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public string TargetRole { get; set; } // "Admin", "Staff", "Customer"
        public bool IsGlobal { get; set; } // optional

        // Navigations
        public AppUser User { get; set; }
    }
}
