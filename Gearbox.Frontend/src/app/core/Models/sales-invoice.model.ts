export interface SalesInvoice {
  id: string;
  customerId: string;
  staffId: string;
  appointmentId?: string;
  totalAmount: number;
  discountAmount: number;
  createdAt: string;
  items: SalesInvoiceItem[];
}

export interface SalesInvoiceItem {
  id?: string;
  salesServicesInvoiceId?: string;
  partId?: string;
  serviceDetailsId?: string;
  type: string; // "Part" or "Service"
  quantity: number;
  unitPrice: number;
}

export interface NewSalesInvoice {
  customerId: string;
  staffId: string;
  appointmentId?: string;
  totalAmount: number;
  discountAmount: number;
  items: SalesInvoiceItem[];
}
