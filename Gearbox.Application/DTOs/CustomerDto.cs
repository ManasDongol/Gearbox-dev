using System;

namespace Gearbox.Application.DTOs
{
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal PendingCredits { get; set; }
        public DateTime RegisteredSince { get; set; }
    }
}
