import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Navmenu } from '../../shared/components/navmenu/navmenu';
import { Topbar } from '../../shared/components/topbar/topbar';
import { CustomerService } from '../../core/services/customer/customer.service';
import { Customer, NewCustomer } from '../../core/models/customer.model';
import {
  NewVehicle,
  Vehicle,
  VehicleService,
} from '../../core/services/vehicle/vehicle.service';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { ToastService } from '../../shared/components/toast/toast.service';
import { Spinner } from '../../shared/components/spinner/spinner';
import { ConfirmCardService } from '../../shared/components/confirm-card/confirm-card.service';

@Component({
  selector: 'app-customer-management',
  standalone: true,
  imports: [Navmenu, Topbar, FormsModule, CommonModule,Spinner],
  templateUrl: './customer-management.html',
  styleUrl: './customer-management.css',
})
export class CustomerManagement implements OnInit {
  private customerService = inject(CustomerService);
  private vehicleService = inject(VehicleService);
  private toast = inject(ToastService);
  private confirmCard = inject(ConfirmCardService);

  customers: Customer[] = [];
  filteredCustomers: Customer[] = [];
  customerVehicles: Vehicle[] = [];
  
  searchQuery: string = '';
  private searchSubject = new Subject<string>();
  
  showAddDialog: boolean = false;
  showEditDialog: boolean = false;
  showVehicleDialog: boolean = false;
  isLoading: boolean = false;
  isVehicleLoading: boolean = false;

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
  selectedVehicleCustomer: Customer | null = null;
  editingVehicle: Vehicle | null = null;
  vehicleForm: NewVehicle = this.emptyVehicleForm();

  vehicleTypes = [
    { label: 'Car', value: 1 },
    { label: 'Bike', value: 2 },
    { label: 'Truck', value: 3 },
    { label: 'Bus', value: 4 },
  ];

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
        this.toast.error('Unable to load customers', 'Please try again.');
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
        this.toast.success('Customer added', 'The customer account was created.');
      },
      error: (err) => {
        console.error('Error registering customer', err);
        this.toast.error('Unable to add customer', 'Please check the details and try again.');
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
        this.toast.success('Customer updated', 'The customer changes were saved.');
      },
      error: (err) => {
        console.error('Error updating customer', err);
        this.toast.error('Unable to update customer', 'Please try again.');
      }
    });
  }

  async deleteCustomer(userId: string) {
    const confirmed = await this.confirmCard.confirm({
      title: 'Delete customer?',
      message: 'Are you sure you want to delete this customer?',
      confirmText: 'OK',
    });
    if (!confirmed) return;

    this.customerService.delete(userId).subscribe({
      next: () => {
        this.customers = this.customers.filter(c => c.userId !== userId);
        this.applyFilter();
        if (this.selectedVehicleCustomer?.userId === userId) {
          this.closeVehiclesPanel();
        }
        this.toast.success('Customer deleted', 'The customer was removed.');
      },
      error: (err) => {
        console.error('Error deleting customer', err);
        this.toast.error('Unable to delete customer', 'Please try again.');
      }
    });
  }

  openVehiclesPanel(customer: Customer) {
    this.selectedVehicleCustomer = customer;
    this.loadCustomerVehicles(customer.userId);
  }

  closeVehiclesPanel() {
    this.selectedVehicleCustomer = null;
    this.customerVehicles = [];
    this.closeVehicleDialog();
  }

  loadCustomerVehicles(customerId: string) {
    this.isVehicleLoading = true;
    this.vehicleService.getByCustomerId(customerId).subscribe({
      next: (vehicles) => {
        this.customerVehicles = vehicles;
        this.isVehicleLoading = false;
      },
      error: (err) => {
        console.error('Error loading customer vehicles', err);
        this.customerVehicles = [];
        this.isVehicleLoading = false;
        this.toast.error('Unable to load vehicles', 'Please try again.');
      }
    });
  }

  openAddVehicleDialog() {
    if (!this.selectedVehicleCustomer) return;

    this.editingVehicle = null;
    this.vehicleForm = {
      ...this.emptyVehicleForm(),
      customerId: this.selectedVehicleCustomer.userId,
    };
    this.showVehicleDialog = true;
  }

  openEditVehicleDialog(vehicle: Vehicle) {
    this.editingVehicle = vehicle;
    this.vehicleForm = {
      customerId: vehicle.customerId,
      numberPlate: this.getVehiclePlate(vehicle),
      make: vehicle.make,
      model: vehicle.model,
      year: vehicle.year,
      vin: vehicle.vin ?? '',
      vehicleType: Number(vehicle.vehicleType ?? 1),
    };
    this.showVehicleDialog = true;
  }

  closeVehicleDialog() {
    this.showVehicleDialog = false;
    this.editingVehicle = null;
    this.vehicleForm = this.emptyVehicleForm();
  }

  saveVehicle() {
    if (!this.selectedVehicleCustomer || !this.vehicleForm.make || !this.vehicleForm.model || !this.vehicleForm.numberPlate) {
      return;
    }

    const payload: NewVehicle = {
      ...this.vehicleForm,
      customerId: this.selectedVehicleCustomer.userId,
      year: Number(this.vehicleForm.year),
      vehicleType: Number(this.vehicleForm.vehicleType),
    };

    this.isVehicleLoading = true;

    if (this.editingVehicle) {
      this.vehicleService.update(this.editingVehicle.id, {
        id: this.editingVehicle.id,
        ...payload,
      }).subscribe({
        next: () => {
          this.loadCustomerVehicles(this.selectedVehicleCustomer!.userId);
          this.closeVehicleDialog();
          this.toast.success('Vehicle updated', 'The vehicle changes were saved.');
        },
        error: (err) => {
          console.error('Error updating vehicle', err);
          this.isVehicleLoading = false;
          this.toast.error('Unable to update vehicle', 'Please try again.');
        }
      });
      return;
    }

    this.vehicleService.add(payload).subscribe({
      next: () => {
        this.loadCustomerVehicles(this.selectedVehicleCustomer!.userId);
        this.closeVehicleDialog();
        this.toast.success('Vehicle added', 'The vehicle was linked to the customer.');
      },
      error: (err) => {
        console.error('Error adding vehicle', err);
        this.isVehicleLoading = false;
        this.toast.error('Unable to add vehicle', 'Please check the details and try again.');
      }
    });
  }

  async deleteVehicle(vehicle: Vehicle) {
    const confirmed = await this.confirmCard.confirm({
      title: 'Delete vehicle?',
      message: `Delete ${vehicle.make} ${vehicle.model}?`,
      confirmText: 'OK',
    });
    if (!confirmed) return;

    this.isVehicleLoading = true;
    this.vehicleService.delete(vehicle.id).subscribe({
      next: () => {
        this.customerVehicles = this.customerVehicles.filter(v => v.id !== vehicle.id);
        this.isVehicleLoading = false;
        this.toast.success('Vehicle deleted', 'The vehicle was removed.');
      },
      error: (err) => {
        console.error('Error deleting vehicle', err);
        this.isVehicleLoading = false;
        this.toast.error('Unable to delete vehicle', 'Please try again.');
      }
    });
  }

  getVehiclePlate(vehicle: Vehicle): string {
    return vehicle.numberPlate || vehicle.licensePlate || 'No plate';
  }

  getVehicleTypeLabel(value: number | string | undefined): string {
    const normalized = Number(value);
    return this.vehicleTypes.find(type => type.value === normalized)?.label ?? 'Vehicle';
  }

  private emptyVehicleForm(): NewVehicle {
    return {
      customerId: '',
      numberPlate: '',
      make: '',
      model: '',
      year: new Date().getFullYear(),
      vin: '',
      vehicleType: 1,
    };
  }

}
