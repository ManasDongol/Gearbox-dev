using System;
using System.Collections.Generic;

namespace Gearbox.Application.DTOs
{
    public class SalesServicesInvoiceDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid StaffId { get; set; }
        public Guid? AppointmentId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<SalesServicesInvoiceItemDto> Items { get; set; } = new List<SalesServicesInvoiceItemDto>();
    }

    public class SalesServicesInvoiceItemDto
    {
        public Guid Id { get; set; }
        public Guid SalesServicesInvoiceId { get; set; }
        public Guid? PartId { get; set; }
        public Guid? ServiceDetailsId { get; set; }
        public string Type { get; set; } // "Part" or "Service"
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
