using System;
using Gearbox.Domain.Entities;

namespace Gearbox.Application.DTOs
{
    public class ServiceReviewDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? AppointmentId { get; set; }
        public Guid? ServiceId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; }
        
        
    }
}
