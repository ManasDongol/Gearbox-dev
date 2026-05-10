import { Component, OnInit, inject } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Navmenu } from '../../shared/components/navmenu/navmenu';
import { Topbar } from '../../shared/components/topbar/topbar';
import { Auth } from '../../core/services/auth/auth';
import { AppointmentService } from '../../core/services/appointment/appointment.service';
import { CustomerService } from '../../core/services/customer/customer.service';
import { VehicleService, Vehicle } from '../../core/services/vehicle/vehicle.service';
import { Appointment, NewAppointment } from '../../core/models/appointment.model';
import { Customer } from '../../core/models/customer.model';
import { ToastService } from '../../shared/components/toast/toast.service';
import { Spinner } from '../../shared/components/spinner/spinner';

@Component({
  selector: 'app-my-appointments',
  standalone: true,
  imports: [CommonModule, FormsModule, DatePipe, Navmenu, Topbar],
  templateUrl: './my-appointments.html',
  styleUrl: './my-appointments.css',
})
export class MyAppointments implements OnInit {
  private auth = inject(Auth);
  private appointmentService = inject(AppointmentService);
  private customerService = inject(CustomerService);
  private vehicleService = inject(VehicleService);

  customer: Customer | null = null;
  appointments: Appointment[] = [];
  vehicles: Vehicle[] = [];
  isLoading = true;
  showDialog = false;
  editingAppointment: Appointment | null = null;
  appointmentForm = this.emptyAppointmentForm();

  ngOnInit() {
    this.customerService.getAll().subscribe({
      next: (customers) => {
        const userId = this.auth.user?.userId;
        this.customer =
          customers.find((customer) => customer.userId === userId || customer.userId === userId) ?? null;
        this.loadData();
      },
      error: (err) => {
        console.error('Error loading customer profile', err);
        this.loadData();
      },
    });
  }

  loadData() {
    const customerId = this.customer?.userId;
    const userId = this.auth.user?.userId;

    this.vehicleService.getAll().subscribe({
      next: (vehicles) => {
        this.vehicles = vehicles.filter(
          (vehicle) => vehicle.customerId === customerId || vehicle.customerId === userId,
        );
      },
      error: (err) => console.error('Error loading appointment vehicles', err),
    });

    this.appointmentService.getAll().subscribe({
      next: (appointments) => {
        this.appointments = appointments
          .filter(
            (appointment) =>
              appointment.customerId === customerId || appointment.customerId === userId,
          )
          .sort(
            (a, b) =>
              new Date(b.appointmentDate).getTime() - new Date(a.appointmentDate).getTime(),
          );
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading appointments', err);
        this.isLoading = false;
      },
    });
  }

  getVehicleName(vehicleId: string): string {
    const vehicle = this.vehicles.find((v) => v.id === vehicleId);
    return vehicle ? `${vehicle.make} ${vehicle.model} (${vehicle.licensePlate})` : 'Vehicle';
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

  openRequestDialog() {
    this.editingAppointment = null;
    this.appointmentForm = this.emptyAppointmentForm();
    this.showDialog = true;
  }

  openEditDialog(appointment: Appointment) {
    const appointmentDate = new Date(appointment.appointmentDate);
    this.editingAppointment = appointment;
    this.appointmentForm = {
      vehicleId: appointment.vehicleId,
      date: appointmentDate.toISOString().split('T')[0],
      time: appointmentDate.toTimeString().slice(0, 5),
      notes: appointment.notes ?? '',
    };
    this.showDialog = true;
  }

  closeDialog() {
    this.showDialog = false;
    this.editingAppointment = null;
    this.appointmentForm = this.emptyAppointmentForm();
  }

  saveAppointment() {
    if (!this.customer?.userId || !this.appointmentForm.vehicleId || !this.appointmentForm.date) return;

    const appointmentDate = new Date(
      `${this.appointmentForm.date}T${this.appointmentForm.time || '09:00'}:00`,
    );

    if (this.editingAppointment) {
      const payload: Appointment = {
        ...this.editingAppointment,
        vehicleId: this.appointmentForm.vehicleId,
        appointmentDate: appointmentDate.toISOString(),
        notes: this.appointmentForm.notes,
        status: 'Pending',
      };

      this.appointmentService.update(payload.id, payload).subscribe({
        next: () => {
          this.loadData();
          this.closeDialog();
        },
        error: (err) => console.error('Error updating appointment request', err),
      });
      return;
    }

    const payload: NewAppointment = {
      customerId: this.customer.userId,
      vehicleId: this.appointmentForm.vehicleId,
      appointmentDate: appointmentDate.toISOString(),
      status: 'Pending',
      notes: this.appointmentForm.notes,
      createdDate: new Date().toISOString(),
    };

    this.appointmentService.add(payload).subscribe({
      next: () => {
        this.loadData();
        this.closeDialog();
      },
      error: (err) => console.error('Error creating appointment request', err),
    });
  }

  cancelAppointment(appointment: Appointment) {
    if (!confirm('Cancel this appointment request?')) return;

    this.appointmentService.delete(appointment.id).subscribe({
      next: () => {
        this.appointments = this.appointments.filter((item) => item.id !== appointment.id);
      },
      error: (err) => console.error('Error cancelling appointment', err),
    });
  }

  private emptyAppointmentForm() {
    return {
      vehicleId: '',
      date: '',
      time: '09:00',
      notes: '',
    };
  }
}
