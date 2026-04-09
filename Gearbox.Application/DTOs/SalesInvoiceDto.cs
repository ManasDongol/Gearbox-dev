using System;

namespace Gearbox.Application.DTOs
{
    public class SalesInvoiceDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid StaffId { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public bool IsLoyaltyDiscountApplied { get; set; }
        public bool IsPaid { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
