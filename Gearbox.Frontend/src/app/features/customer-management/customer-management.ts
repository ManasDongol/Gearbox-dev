import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Navmenu } from '../../shared/components/navmenu/navmenu';
import { Topbar } from '../../shared/components/topbar/topbar';
import { CustomerService } from '../../core/services/customer/customer.service';
import { Customer, NewCustomer } from '../../core/models/customer.model';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';

@Component({
  selector: 'app-customer-management',
  standalone: true,
  imports: [Navmenu, Topbar, FormsModule, CommonModule],
  templateUrl: './customer-management.html',
  styleUrl: './customer-management.css',
})
export class CustomerManagement implements OnInit {
  private customerService = inject(CustomerService);

  customers: Customer[] = [];
  filteredCustomers: Customer[] = [];
  
  searchQuery: string = '';
  private searchSubject = new Subject<string>();
  
  showAddDialog: boolean = false;
  showEditDialog: boolean = false;
  isLoading: boolean = false;

  newCustomer: NewCustomer = {
    userName: '',
    firstName: '',
    lastName: '',
    phoneNumber: '',
    email: '',
    password: '',
    address: '',
    totalSpent: 0,
    pendingCredits: 0
  };

  selectedCustomer: Customer | null = null;

  ngOnInit() {
    this.loadCustomers();

    // Setup search debounce
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(query => {
      this.applyFilter();
    });
  }

  loadCustomers() {
    this.isLoading = true;
    this.customerService.getAll().subscribe({
      next: (data) => {
        console.log(data)
        this.customers = data;
        this.applyFilter();
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading customers', err);
        this.isLoading = false;
      }
    });
  }

  onSearchChange(query: string) {
    this.searchSubject.next(query);
  }

  applyFilter() {
    const query = this.searchQuery.toLowerCase().trim();
    this.filteredCustomers = this.customers.filter((c) => {
      return (
        !query ||
        c.firstName.toLowerCase().includes(query) ||
        c.lastName.toLowerCase().includes(query) ||
        c.phoneNumber.toLowerCase().includes(query) ||
        c.address.toLowerCase().includes(query)
      );
    });
  }

  openAddDialog() {
    this.showAddDialog = true;
  }

  closeAddDialog() {
    this.showAddDialog = false;
    this.resetForm();
  }

  openEditDialog(customer: Customer) {
    this.selectedCustomer = { ...customer };
    this.showEditDialog = true;
  }

  closeEditDialog() {
    this.showEditDialog = false;
    this.selectedCustomer = null;
  }

  resetForm() {
    this.newCustomer = {
      userName: '',
      firstName: '',
      lastName: '',
      phoneNumber: '',
      email: '',
      password: '',
      address: '',
      totalSpent: 0,
      pendingCredits: 0
    };
  }

  registerCustomer() {
    if (!this.newCustomer.userName || !this.newCustomer.email || !this.newCustomer.password) return;

    this.customerService.add(this.newCustomer).subscribe({
      next: () => {
        this.loadCustomers();
        this.closeAddDialog();
      },
      error: (err) => {
        console.error('Error registering customer', err);
      }
    });
  }

  updateCustomer() {
    if (!this.selectedCustomer) return;

    this.customerService.update(this.selectedCustomer.userId, this.selectedCustomer).subscribe({
      next: () => {
        const index = this.customers.findIndex(c => c.userId === this.selectedCustomer?.userId);
        if (index !== -1 && this.selectedCustomer) {
          this.customers[index] = { ...this.selectedCustomer };
          this.applyFilter();
        }
        this.closeEditDialog();
      },
      error: (err) => {
        console.error('Error updating customer', err);
      }
    });
  }

  deleteCustomer(userId: string) {
    if (confirm('Are you sure you want to delete this customer?')) {
      this.customerService.delete(userId).subscribe({
        next: () => {
          this.customers = this.customers.filter(c => c.userId !== userId);
          this.applyFilter();
        },
        error: (err) => {
          console.error('Error deleting customer', err);
        }
      });
    }
  }
}
