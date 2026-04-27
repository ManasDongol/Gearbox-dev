using System;

namespace Gearbox.Application.DTOs.Vendor
{
    public class VendorDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Email { get; set; }
        public required string Address { get; set; }
    }
}
