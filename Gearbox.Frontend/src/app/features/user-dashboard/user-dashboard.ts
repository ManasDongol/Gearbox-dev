import { Component, OnInit, inject } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { Router } from '@angular/router';
import { Navmenu } from '../../shared/components/navmenu/navmenu';
import { Topbar } from '../../shared/components/topbar/topbar';
import { Auth } from '../../core/services/auth/auth';
import { CustomerService } from '../../core/services/customer/customer.service';
import { VehicleService, Vehicle } from '../../core/services/vehicle/vehicle.service';
import { AppointmentService } from '../../core/services/appointment/appointment.service';
import { SalesInvoiceService } from '../../core/services/sales-invoice/sales-invoice.service';
import { PartRequestService } from '../../core/services/part-request/part-request.service';
import { Customer } from '../../core/models/customer.model';
import { Appointment } from '../../core/models/appointment.model';
import { SalesInvoice } from '../../core/models/sales-invoice.model';
import { PartRequest } from '../../core/models/part-request.model';
import { ToastService } from '../../shared/components/toast/toast.service';
import { Spinner } from '../../shared/components/spinner/spinner';

@Component({
  selector: 'app-user-dashboard',
  standalone: true,
  imports: [CommonModule, DatePipe, Navmenu, Topbar, Spinner],
  templateUrl: './user-dashboard.html',
  styleUrl: './user-dashboard.css',
})
export class UserDashboard implements OnInit {
  private auth = inject(Auth);
  private router = inject(Router);
  private customerService = inject(CustomerService);
  private vehicleService = inject(VehicleService);
  private appointmentService = inject(AppointmentService);
  private salesInvoiceService = inject(SalesInvoiceService);
  private partRequestService = inject(PartRequestService);
  private toast = inject(ToastService);

  customer: Customer | null = null;
  vehicles: Vehicle[] = [];
  appointments: Appointment[] = [];
  invoices: SalesInvoice[] = [];
  requests: PartRequest[] = [];
  isLoading = true;

  ngOnInit() {
    this.loadDashboard();
  }

  get totalSpent(): number {
    return this.invoices.reduce((sum, invoice) => sum + invoice.totalAmount, 0);
  }

  get pendingCredits(): number {
    return this.invoices
      .filter((invoice) => !invoice.paymentStatus)
      .reduce((sum, invoice) => sum + invoice.totalAmount, 0);
  }

  get upcomingAppointments(): Appointment[] {
    const now = new Date();
    return this.appointments
      .filter((appointment) => new Date(appointment.appointmentDate) >= now)
      .sort(
        (a, b) =>
          new Date(a.appointmentDate).getTime() - new Date(b.appointmentDate).getTime(),
      )
      .slice(0, 4);
  }

  get recentVehicles(): Vehicle[] {
    return this.vehicles.slice(0, 5);
  }

  loadDashboard() {
    this.isLoading = true;
    this.customerService.getAll().subscribe({
      next: (customers) => {
        const userId = this.auth.user?.userId;
        this.customer =
          customers.find((customer) => customer.userId === userId || customer.userId === userId) ?? null;
        this.loadCustomerData();
      },
      error: (err) => {
        console.error('Error loading customer dashboard profile', err);
        this.toast.error('Unable to load profile', 'Dashboard data may be incomplete.');
        this.loadCustomerData();
      },
    });
  }

  loadCustomerData() {
    const customerId = this.customer?.userId;
    const userId = this.auth.user?.userId;
    console.log(this.customer);

    this.vehicleService.getAll().subscribe({
      next: (vehicles) => {
        this.vehicles = vehicles.filter(
          (vehicle) => vehicle.customerId === customerId || vehicle.customerId === userId,
        );
      },
      error: (err) => {
        console.error('Error loading customer vehicles', err);
        this.toast.error('Unable to load vehicles', 'Dashboard vehicle data may be incomplete.');
      },
    });

    this.appointmentService.getAll().subscribe({
      next: (appointments) => {
        this.appointments = appointments.filter(
          (appointment) =>
            appointment.customerId === customerId || appointment.customerId === userId,
        );
      },
      error: (err) => {
        console.error('Error loading customer appointments', err);
        this.toast.error('Unable to load appointments', 'Dashboard appointment data may be incomplete.');
      },
    });

    this.salesInvoiceService.getAll().subscribe({
      next: (invoices) => {
        this.invoices = invoices.filter(
          (invoice) => invoice.customerId === customerId || invoice.customerId === userId,
        );
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading customer invoices', err);
        this.toast.error('Unable to load invoices', 'Please try again.');
        this.isLoading = false;
      },
    });

    this.partRequestService.getAll().subscribe({
      next: (requests) => {
        this.requests = requests.filter(
          (request) => request.customerId === customerId || request.customerId === userId,
        );
      },
      error: (err) => {
        console.error('Error loading customer part requests', err);
        this.toast.error('Unable to load part requests', 'Dashboard request data may be incomplete.');
      },
    });
  }

  getVehicleName(vehicleId: string): string {
    const vehicle = this.vehicles.find((v) => v.id === vehicleId);
    return vehicle ? `${vehicle.make} ${vehicle.model}` : 'Vehicle';
  }

  getVehiclePlate(vehicle: Vehicle): string {
    return vehicle.numberPlate || vehicle.licensePlate || 'No plate';
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'Confirmed':
        return 'bg-blue-50 text-blue-700';
      case 'Completed':
        return 'bg-green-50 text-green-700';
      case 'Cancelled':
        return 'bg-red-50 text-red-700';
      default:
        return 'bg-yellow-50 text-yellow-700';
    }
  }

  openInvoices() {
    this.router.navigate(['/my-invoices']);
  }

  openVehicles() {
    this.router.navigate(['/my-vehicles']);
  }

  openAppointments() {
    this.router.navigate(['/my-appointments']);
  }

  openRequests() {
    this.router.navigate(['/requests']);
  }

  payPendingCredits() {
    this.toast.info('Payment flow', 'Payment flow will open here.');
  }
}
