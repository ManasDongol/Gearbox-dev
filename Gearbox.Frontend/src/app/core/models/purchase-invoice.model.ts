export interface PurchaseInvoice {
  id: string;
  vendorId: string;
  invoiceNumber: string;
  totalAmount: number;
  createdDate: string;
  items?: PurchaseInvoiceItem[];
}

export interface PurchaseInvoiceItem {
  id?: string;
  purchaseInvoiceId?: string;
  partId: string;
  quantity: number;
  costPrice: number;
}

export interface NewPurchaseInvoice {
  vendorId: string;
  invoiceNumber: string;
  totalAmount: number;
  items: NewPurchaseInvoiceItem[];
}

export interface NewPurchaseInvoiceItem {
  partId: string;
  quantity: number;
  costPrice: number;
}
