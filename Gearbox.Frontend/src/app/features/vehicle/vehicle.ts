import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Navmenu } from '../../shared/components/navmenu/navmenu';
import { Topbar } from '../../shared/components/topbar/topbar';
import { Auth } from '../../core/services/auth/auth';
import { CustomerService } from '../../core/services/customer/customer.service';
import {
  NewVehicle,
  VehicleService,
  Vehicle as CustomerVehicle,
} from '../../core/services/vehicle/vehicle.service';
import { Customer } from '../../core/models/customer.model';
import { ToastService } from '../../shared/components/toast/toast.service';
import { Spinner } from '../../shared/components/spinner/spinner';

@Component({
  selector: 'app-vehicle',
  standalone: true,
  imports: [CommonModule, FormsModule, Navmenu, Topbar, Spinner],
  templateUrl: './vehicle.html',
  styleUrl: './vehicle.css',
})
export class Vehicle implements OnInit {
  private auth = inject(Auth);
  private customerService = inject(CustomerService);
  private vehicleService = inject(VehicleService);
  private toast = inject(ToastService);

  customer: Customer | null = null;
  vehicles: CustomerVehicle[] = [];
  isLoading = true;
  showDialog = false;
  editingVehicle: CustomerVehicle | null = null;
  vehicleForm: NewVehicle = this.emptyVehicleForm();

  vehicleTypes = [
    { label: 'Car', value: 1 },
    { label: 'Bike', value: 2 },
    { label: 'Truck', value: 3 },
    { label: 'Bus', value: 4 },
  ];

  ngOnInit() {
    this.customerService.getAll().subscribe({
      next: (customers) => {
        const userId = this.auth.user?.userId;
        this.customer =
          customers.find((customer) => customer.userId === userId || customer.userId === userId) ?? null;
        this.loadVehicles();
      },
      error: (err) => {
        console.error('Error loading customer profile', err);
        this.toast.error('Unable to load customer profile', 'Vehicle ownership may be incomplete.');
        this.loadVehicles();
      },
    });
  }

  loadVehicles() {
    const customerId = this.customer?.userId;
    const userId = this.auth.user?.userId;

    this.vehicleService.getAll().subscribe({
      next: (vehicles) => {
        this.vehicles = vehicles.filter(
          (vehicle) => vehicle.customerId === customerId || vehicle.customerId === userId,
        );
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading vehicles', err);
        this.toast.error('Unable to load vehicles', 'Please try again.');
        this.isLoading = false;
      },
    });
  }

  getPlate(vehicle: CustomerVehicle): string {
    return vehicle.numberPlate || vehicle.licensePlate || 'No plate';
  }

  getVehicleTypeLabel(value: number | string | undefined): string {
    const normalized = Number(value);
    return this.vehicleTypes.find((type) => type.value === normalized)?.label ?? 'Vehicle';
  }

  openAddDialog() {
    this.editingVehicle = null;
    this.vehicleForm = this.emptyVehicleForm();
    this.showDialog = true;
  }

  openEditDialog(vehicle: CustomerVehicle) {
    this.editingVehicle = vehicle;
    this.vehicleForm = {
      customerId: vehicle.customerId,
      numberPlate: this.getPlate(vehicle),
      make: vehicle.make,
      model: vehicle.model,
      year: vehicle.year,
      vin: vehicle.vin ?? '',
      vehicleType: Number(vehicle.vehicleType ?? 1),
    };
    this.showDialog = true;
  }

  closeDialog() {
    this.showDialog = false;
    this.editingVehicle = null;
    this.vehicleForm = this.emptyVehicleForm();
  }

  saveVehicle() {
    if (!this.customer?.userId || !this.vehicleForm.make || !this.vehicleForm.model || !this.vehicleForm.numberPlate) {
      return;
    }

    const payload: NewVehicle = {
      ...this.vehicleForm,
      customerId: this.customer.userId,
      year: Number(this.vehicleForm.year),
      vehicleType: Number(this.vehicleForm.vehicleType),
    };

    if (this.editingVehicle) {
      this.isLoading = true;
      this.vehicleService
        .update(this.editingVehicle.id, {
          id: this.editingVehicle.id,
          ...payload,
        })
        .subscribe({
          next: () => {
            this.loadVehicles();
            this.closeDialog();
            this.toast.success('Vehicle updated', 'The vehicle changes were saved.');
          },
          error: (err) => {
            console.error('Error updating vehicle', err);
            this.isLoading = false;
            this.toast.error('Unable to update vehicle', 'Please try again.');
          },
        });
      return;
    }

    this.isLoading = true;
    this.vehicleService.add(payload).subscribe({
      next: () => {
        this.loadVehicles();
        this.closeDialog();
        this.toast.success('Vehicle added', 'The vehicle was saved.');
      },
      error: (err) => {
        console.error('Error adding vehicle', err);
        this.isLoading = false;
        this.toast.error('Unable to add vehicle', 'Please check the details and try again.');
      },
    });
  }

  deleteVehicle(vehicle: CustomerVehicle) {
    if (!confirm(`Delete ${vehicle.make} ${vehicle.model}?`)) return;

    this.isLoading = true;
    this.vehicleService.delete(vehicle.id).subscribe({
      next: () => {
        this.vehicles = this.vehicles.filter((item) => item.id !== vehicle.id);
        this.isLoading = false;
        this.toast.success('Vehicle deleted', 'The vehicle was removed.');
      },
      error: (err) => {
        console.error('Error deleting vehicle', err);
        this.isLoading = false;
        this.toast.error('Unable to delete vehicle', 'Please try again.');
      },
    });
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
