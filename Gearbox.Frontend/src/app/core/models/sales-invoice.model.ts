export interface SalesInvoice {
  id: string;
  customerId: string;
  staffId: string;
  appointmentId?: string;
  totalAmount: number;
  discountAmount: number;
  paymentStatus: boolean;
  createdAt: string;
  items: SalesInvoiceItem[];
}

export interface SalesInvoiceItem {
  id?: string;
  salesServicesInvoiceId?: string;
  partId?: string;
  serviceDetailsId?: string;
  serviceId?: string;
  vehicleId?: string;
  type: string; // "Part" or "Service"
  itemName?: string;
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
