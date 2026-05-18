import { Component, OnInit, inject } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { Navmenu } from '../../shared/components/navmenu/navmenu';
import { Topbar } from '../../shared/components/topbar/topbar';
import { Auth } from '../../core/services/auth/auth';
import { CustomerService } from '../../core/services/customer/customer.service';
import { SalesInvoiceService } from '../../core/services/sales-invoice/sales-invoice.service';
import {
  ServiceHistory,
  ServiceHistoryService,
} from '../../core/services/service-history/service-history.service';
import { Customer } from '../../core/models/customer.model';
import { SalesInvoice } from '../../core/models/sales-invoice.model';
import { ToastService } from '../../shared/components/toast/toast.service';
import { Spinner } from '../../shared/components/spinner/spinner';

@Component({
  selector: 'app-my-invoices',
  standalone: true,
  imports: [CommonModule, DatePipe, Navmenu, Topbar,Spinner],
  templateUrl: './my-invoices.html',
  styleUrl: './my-invoices.css',
})
export class MyInvoices implements OnInit {
  private auth = inject(Auth);
  private customerService = inject(CustomerService);
  private salesInvoiceService = inject(SalesInvoiceService);
  private serviceHistoryService = inject(ServiceHistoryService);
  private toast = inject(ToastService);

  customer: Customer | null = null;
  invoices: SalesInvoice[] = [];
  serviceHistory: ServiceHistory[] = [];
  selectedInvoice: SalesInvoice | null = null;
  isLoading = true;
  isLoadingServiceHistory = true;
  isLoadingInvoiceItems = false;
  payingInvoiceId: string | null = null;

  ngOnInit() {
    this.customerService.getAll().subscribe({
      next: (customers) => {
        const userId = this.auth.user?.userId;
        this.customer =
          customers.find((customer) => customer.userId === userId || customer.userId === userId) ?? null;
        this.loadInvoices();
        this.loadServiceHistory();
      },
      error: (err) => {
        console.error('Error loading customer profile', err);
        this.loadInvoices();
        this.loadServiceHistory();
      },
    });
  }

  get totalSpent(): number {
    return this.invoices.reduce((sum, invoice) => sum + invoice.totalAmount, 0);
  }

  get pendingCredits(): number {
    return this.invoices
      .filter((invoice) => !invoice.paymentStatus)
      .reduce((sum, invoice) => sum + invoice.totalAmount, 0);
  }

  loadInvoices() {
    const customerId = this.customer?.userId;
    const userId = this.auth.user?.userId;

    this.salesInvoiceService.getAll().subscribe({
      next: (invoices) => {
        this.invoices = invoices
          .filter((invoice) => invoice.customerId === customerId || invoice.customerId === userId)
          .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading invoices', err);
        this.isLoading = false;
      },
    });
  }

  loadServiceHistory() {
    const customerId = this.customer?.userId;
    const userId = this.auth.user?.userId;

    this.serviceHistoryService.getAll().subscribe({
      next: (history) => {
        this.serviceHistory = history
          .filter((item) => item.customerId === customerId || item.customerId === userId)
          .sort((a, b) => new Date(b.serviceDate).getTime() - new Date(a.serviceDate).getTime());
        this.isLoadingServiceHistory = false;
      },
      error: (err) => {
        console.error('Error loading service history', err);
        this.toast.error('Service history failed', 'Could not load service history.');
        this.isLoadingServiceHistory = false;
      },
    });
  }

  payInvoice(invoice: SalesInvoice) {
    if (invoice.paymentStatus || this.payingInvoiceId) return;

    this.payingInvoiceId = invoice.id;
    this.salesInvoiceService.markAsPaid(invoice.id).subscribe({
      next: (updatedInvoice) => {
        this.invoices = this.invoices.map((item) =>
          item.id === updatedInvoice.id ? { ...item, paymentStatus: updatedInvoice.paymentStatus } : item
        );

        if (this.selectedInvoice?.id === updatedInvoice.id) {
          this.selectedInvoice = { ...this.selectedInvoice, paymentStatus: updatedInvoice.paymentStatus };
        }

        this.toast.success('Payment updated', 'Invoice marked as paid.');
        this.payingInvoiceId = null;
      },
      error: (err) => {
        console.error('Error updating invoice payment status', err);
        this.toast.error('Payment failed', 'Could not update the invoice payment status.');
        this.payingInvoiceId = null;
      },
    });
  }

  viewInvoice(invoice: SalesInvoice) {
    this.selectedInvoice = invoice;
    this.isLoadingInvoiceItems = true;

    this.salesInvoiceService.getById(invoice.id).subscribe({
      next: (invoiceDetails) => {
        this.selectedInvoice = invoiceDetails;
        this.isLoadingInvoiceItems = false;
      },
      error: (err) => {
        console.error('Error loading invoice items', err);
        this.toast.error('Invoice details failed', 'Could not load invoice items.');
        this.isLoadingInvoiceItems = false;
      },
    });
  }

  closeInvoiceDetails() {
    this.selectedInvoice = null;
    this.isLoadingInvoiceItems = false;
  }

  itemTotal(item: SalesInvoice['items'][number]): number {
    return item.quantity * item.unitPrice;
  }
}
