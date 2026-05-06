using System;

namespace Gearbox.Application.DTOs.Customer
{
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal PendingCredits { get; set; }
        public DateTime RegisteredSince { get; set; }
    }
}
