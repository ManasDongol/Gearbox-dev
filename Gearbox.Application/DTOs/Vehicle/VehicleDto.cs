using System;
using Gearbox.Domain.ENUMs;

namespace Gearbox.Application.DTOs
{
    public class VehicleDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string NumberPlate { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string VIN { get; set; }
        public VehicleType VehicleType { get; set; }

    }
}
