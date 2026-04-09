using System;
using Gearbox.Domain.Entities;

namespace Gearbox.Application.DTOs
{
    public class ServiceBillDto
    {
        public Guid Id { get; set; }
        public Guid? AppointmentId { get; set; }
        public Guid? ServiceHistoryId { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsPaid { get; set; }
        public DateTime CreatedDate { get; set; }
       
    }
}
