import { Component, OnInit, inject } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
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
import { ServiceReviewService } from '../../core/services/service-review/service-review.service';
import { ServiceReview } from '../../core/models/service-review.model';

@Component({
  selector: 'app-my-invoices',
  standalone: true,
  imports: [CommonModule, DatePipe, FormsModule, Navmenu, Topbar,Spinner],
  templateUrl: './my-invoices.html',
  styleUrl: './my-invoices.css',
})
export class MyInvoices implements OnInit {
  private auth = inject(Auth);
  private customerService = inject(CustomerService);
  private salesInvoiceService = inject(SalesInvoiceService);
  private serviceHistoryService = inject(ServiceHistoryService);
  private serviceReviewService = inject(ServiceReviewService);
  private toast = inject(ToastService);

  customer: Customer | null = null;
  invoices: SalesInvoice[] = [];
  serviceHistory: ServiceHistory[] = [];
  serviceReviews: ServiceReview[] = [];
  selectedInvoice: SalesInvoice | null = null;
  selectedServiceHistory: ServiceHistory | null = null;
  isLoading = true;
  isLoadingServiceHistory = true;
  isLoadingReviews = true;
  isLoadingInvoiceItems = false;
  payingInvoiceId: string | null = null;
  isSubmittingReview = false;
  reviewedServiceHistoryIds = new Set<string>();
  reviewForm = {
    rating: 5,
    comment: '',
  };

  ngOnInit() {
    this.customerService.getAll().subscribe({
      next: (customers) => {
        const userId = this.auth.user?.userId;
        this.customer =
          customers.find((customer) => customer.userId === userId || customer.userId === userId) ?? null;
        this.loadInvoices();
        this.loadServiceHistory();
        this.loadServiceReviews();
      },
      error: (err) => {
        console.error('Error loading customer profile', err);
        this.loadInvoices();
        this.loadServiceHistory();
        this.loadServiceReviews();
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

  loadServiceReviews() {
    const customerId = this.customer?.userId;
    const userId = this.auth.user?.userId;

    this.serviceReviewService.getAll().subscribe({
      next: (reviews) => {
        this.serviceReviews = reviews.filter(
          (review) => review.customerId === customerId || review.customerId === userId,
        );
        this.isLoadingReviews = false;
      },
      error: (err) => {
        console.error('Error loading service reviews', err);
        this.toast.error('Reviews failed', 'Could not load your service reviews.');
        this.isLoadingReviews = false;
      },
    });
  }

  hasReviewedService(history: ServiceHistory): boolean {
    if (this.reviewedServiceHistoryIds.has(history.id)) return true;
    if (!history.serviceId) return false;

    return this.serviceReviews.some(
      (review) => review.customerId === history.customerId && review.serviceId === history.serviceId,
    );
  }

  canReviewService(history: ServiceHistory): boolean {
    return !this.isLoadingReviews && !this.hasReviewedService(history);
  }

  openReviewDialog(history: ServiceHistory) {
    this.selectedServiceHistory = history;
    this.reviewForm = {
      rating: 5,
      comment: '',
    };
  }

  closeReviewDialog() {
    this.selectedServiceHistory = null;
    this.reviewForm = {
      rating: 5,
      comment: '',
    };
    this.isSubmittingReview = false;
  }

  submitReview() {
    if (!this.selectedServiceHistory || this.isSubmittingReview) return;

    const rating = Number(this.reviewForm.rating);
    if (rating < 1 || rating > 5) {
      this.toast.error('Invalid rating', 'Choose a rating between 1 and 5.');
      return;
    }

    this.isSubmittingReview = true;
    const history = this.selectedServiceHistory;

    this.serviceReviewService.add({
      customerId: history.customerId,
      serviceId: history.serviceId || null,
      appointmentId: null,
      rating,
      comment: this.reviewForm.comment.trim(),
      reviewDate: new Date().toISOString(),
    }).subscribe({
      next: (review) => {
        this.serviceReviews = [...this.serviceReviews, review];
        this.reviewedServiceHistoryIds.add(history.id);
        this.toast.success('Review submitted', 'Thanks for sharing your service feedback.');
        this.closeReviewDialog();
      },
      error: (err) => {
        console.error('Error submitting service review', err);
        this.toast.error('Review failed', 'Could not submit your review.');
        this.isSubmittingReview = false;
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
