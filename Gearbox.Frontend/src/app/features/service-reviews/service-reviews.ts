import { CommonModule, DatePipe } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { forkJoin } from 'rxjs';
import { Navmenu } from '../../shared/components/navmenu/navmenu';
import { Topbar } from '../../shared/components/topbar/topbar';
import { Spinner } from '../../shared/components/spinner/spinner';
import { ToastService } from '../../shared/components/toast/toast.service';
import { CustomerService } from '../../core/services/customer/customer.service';
import { ServiceService } from '../../core/services/service/service.service';
import { ServiceReviewService } from '../../core/services/service-review/service-review.service';
import { Customer } from '../../core/models/customer.model';
import { Service } from '../../core/models/service.model';
import { ServiceReview } from '../../core/models/service-review.model';

@Component({
  selector: 'app-service-reviews',
  standalone: true,
  imports: [CommonModule, DatePipe, Navmenu, Topbar, Spinner],
  templateUrl: './service-reviews.html',
  styleUrl: './service-reviews.css',
})
export class ServiceReviews implements OnInit {
  private reviewService = inject(ServiceReviewService);
  private customerService = inject(CustomerService);
  private serviceService = inject(ServiceService);
  private toast = inject(ToastService);

  reviews: ServiceReview[] = [];
  customers: Customer[] = [];
  services: Service[] = [];
  isLoading = true;

  ngOnInit() {
    this.loadReviews();
  }

  loadReviews() {
    this.isLoading = true;

    forkJoin({
      reviews: this.reviewService.getAll(),
      customers: this.customerService.getAll(),
      services: this.serviceService.getAll(),
    }).subscribe({
      next: ({ reviews, customers, services }) => {
        this.reviews = reviews.sort(
          (a, b) => new Date(b.reviewDate).getTime() - new Date(a.reviewDate).getTime(),
        );
        this.customers = customers;
        this.services = services;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading service reviews', err);
        this.toast.error('Unable to load reviews', 'Please try again.');
        this.isLoading = false;
      },
    });
  }

  get averageRating(): number {
    if (!this.reviews.length) return 0;
    const total = this.reviews.reduce((sum, review) => sum + review.rating, 0);
    return Math.round((total / this.reviews.length) * 10) / 10;
  }

  getCustomerName(customerId: string): string {
    const customer = this.customers.find((item) => item.userId === customerId);
    return customer ? `${customer.firstName} ${customer.lastName}` : 'Unknown customer';
  }

  getServiceName(serviceId?: string | null): string {
    if (!serviceId) return 'General service';
    return this.services.find((item) => item.id === serviceId)?.name ?? 'Unknown service';
  }

  getRatingLabel(rating: number): string {
    return `${rating} / 5`;
  }

  shortId(id?: string | null): string {
    return id ? id.slice(0, 8) : 'None';
  }
}
