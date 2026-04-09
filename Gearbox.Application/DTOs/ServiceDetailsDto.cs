using System;

namespace Gearbox.Application.DTOs
{
    public class ServiceDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
    }
}
