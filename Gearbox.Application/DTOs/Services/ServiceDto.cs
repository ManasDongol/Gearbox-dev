using System;

namespace Gearbox.Application.DTOs
{
    public class ServiceDto
    {
      public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
