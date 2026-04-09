using System;

namespace Gearbox.Application.DTOs
{
    public class SalesInvoiceItemDto
    {
        public Guid Id { get; set; }
        public Guid SalesInvoiceId { get; set; }
        public Guid PartId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
