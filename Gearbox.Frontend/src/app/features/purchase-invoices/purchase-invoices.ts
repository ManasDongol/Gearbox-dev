import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Navmenu } from '../../shared/components/navmenu/navmenu';

@Component({
  selector: 'app-purchase-invoices',
  imports: [Navmenu, FormsModule],
  templateUrl: './purchase-invoices.html',
  styleUrl: './purchase-invoices.css',
})
export class PurchaseInvoices {
  invoices = [
    {
      id: 1,
      vendorId: 'VND-001',
      invoiceNumber: 'INV-2026-0001',
      totalNumber: 15200.0,
      createdDate: '2026-04-25',
    },
    {
      id: 2,
      vendorId: 'VND-003',
      invoiceNumber: 'INV-2026-0002',
      totalNumber: 8450.5,
      createdDate: '2026-04-24',
    },
    {
      id: 3,
      vendorId: 'VND-007',
      invoiceNumber: 'INV-2026-0003',
      totalNumber: 32000.0,
      createdDate: '2026-04-22',
    },
    {
      id: 4,
      vendorId: 'VND-002',
      invoiceNumber: 'INV-2026-0004',
      totalNumber: 5675.25,
      createdDate: '2026-04-20',
    },
    {
      id: 5,
      vendorId: 'VND-010',
      invoiceNumber: 'INV-2026-0005',
      totalNumber: 19800.0,
      createdDate: '2026-04-18',
    },
  ];

  searchQuery: string = '';

  get filteredInvoices() {
    if (!this.searchQuery.trim()) {
      return this.invoices;
    }
    const query = this.searchQuery.toLowerCase();
    return this.invoices.filter(
      (inv) =>
        inv.vendorId.toLowerCase().includes(query) ||
        inv.invoiceNumber.toLowerCase().includes(query) ||
        inv.id.toString().includes(query)
    );
  }
}
