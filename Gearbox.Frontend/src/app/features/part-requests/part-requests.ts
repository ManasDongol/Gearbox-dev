import { Component, OnInit, inject } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Navmenu } from '../../shared/components/navmenu/navmenu';
import { Topbar } from '../../shared/components/topbar/topbar';
import { Auth } from '../../core/services/auth/auth';
import { CustomerService } from '../../core/services/customer/customer.service';
import { PartRequestService } from '../../core/services/part-request/part-request.service';
import { Customer } from '../../core/models/customer.model';
import { NewPartRequest, PartRequest } from '../../core/models/part-request.model';
import { ToastService } from '../../shared/components/toast/toast.service';
import { Spinner } from '../../shared/components/spinner/spinner';

@Component({
  selector: 'app-part-requests',
  standalone: true,
  imports: [CommonModule, FormsModule, DatePipe, Navmenu, Topbar,Spinner],
  templateUrl: './part-requests.html',
  styleUrl: './part-requests.css',
})
export class PartRequests implements OnInit {
  private auth = inject(Auth);
  private customerService = inject(CustomerService);
  private requestService = inject(PartRequestService);

  customer: Customer | null = null;
  requests: PartRequest[] = [];
  isLoading = true;
  showDialog = false;
  editingRequest: PartRequest | null = null;
  requestForm = this.emptyRequestForm();

  ngOnInit() {
    this.customerService.getAll().subscribe({
      next: (customers) => {
        const userId = this.auth.user?.userId;
        this.customer =
          customers.find((customer) => customer.userId === userId || customer.userId === userId) ?? null;
        this.loadRequests();
      },
      error: (err) => {
        console.error('Error loading customer profile', err);
        this.loadRequests();
      },
    });
  }

  loadRequests() {
    const customerId = this.customer?.userId;
    const userId = this.auth.user?.userId;

    this.requestService.getAll().subscribe({
      next: (requests) => {
        this.requests = requests
          .filter((request) => request.customerId === customerId || request.customerId === userId)
          .sort(
            (a, b) => new Date(b.requestDate).getTime() - new Date(a.requestDate).getTime(),
          );
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading part requests', err);
        this.isLoading = false;
      },
    });
  }

  openAddDialog() {
    this.editingRequest = null;
    this.requestForm = this.emptyRequestForm();
    this.showDialog = true;
  }

  openEditDialog(request: PartRequest) {
    this.editingRequest = request;
    this.requestForm = {
      partName: request.partName,
      description: request.description,
    };
    this.showDialog = true;
  }

  closeDialog() {
    this.showDialog = false;
    this.editingRequest = null;
    this.requestForm = this.emptyRequestForm();
  }

  saveRequest() {
    if (!this.customer?.userId || !this.requestForm.partName) return;

    if (this.editingRequest) {
      const payload: PartRequest = {
        ...this.editingRequest,
        partName: this.requestForm.partName,
        description: this.requestForm.description,
      };

      this.requestService.update(payload.id, payload).subscribe({
        next: () => {
          this.loadRequests();
          this.closeDialog();
        },
        error: (err) => console.error('Error updating part request', err),
      });
      return;
    }

    const payload: NewPartRequest = {
      customerId: this.customer.userId,
      partName: this.requestForm.partName,
      description: this.requestForm.description,
      isFulfilled: false,
      requestDate: new Date().toISOString(),
    };

    this.requestService.add(payload).subscribe({
      next: () => {
        this.loadRequests();
        this.closeDialog();
      },
      error: (err) => console.error('Error creating part request', err),
    });
  }

  deleteRequest(request: PartRequest) {
    if (!confirm(`Delete request for ${request.partName}?`)) return;

    this.requestService.delete(request.id).subscribe({
      next: () => {
        this.requests = this.requests.filter((item) => item.id !== request.id);
      },
      error: (err) => console.error('Error deleting part request', err),
    });
  }

  private emptyRequestForm() {
    return {
      partName: '',
      description: '',
    };
  }
}
