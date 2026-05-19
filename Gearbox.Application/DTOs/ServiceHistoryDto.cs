using System;

namespace Gearbox.Application.DTOs
{
    public class ServiceHistoryDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid VehicleId { get; set; }
        public Guid ServiceId { get; set; }
        public DateTime ServiceDate { get; set; }
        public string Notes { get; set; }
        public decimal TotalCost { get; set; }
    }
}
