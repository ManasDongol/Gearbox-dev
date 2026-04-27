using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gearbox.Domain.Entities
{
    public class Customer
    {
        [Key]
        public Guid UserId { get; set; }
        [StringLength(50)]     
        
       
      
        public string? ProfilePictureUrl { get; set; }
        public decimal TotalSpent { get; set; } = 0; // For tracking high spenders
        public decimal PendingCredits { get; set; } = 0; // For pending credits
        public DateTime RegisteredSince { get; set; } = DateTime.UtcNow;

        // Navigations
        public AppUser User { get; set; } = null!;
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public ICollection<SalesInvoice> SalesInvoices { get; set; } = new List<SalesInvoice>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<ServiceHistory> ServiceHistories { get; set; } = new List<ServiceHistory>();
        public ICollection<ServiceReview> ServiceReviews { get; set; } = new List<ServiceReview>();
        public ICollection<PartRequest> PartRequests { get; set; } = new List<PartRequest>();
    }
}
