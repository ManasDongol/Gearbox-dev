import { Component, OnInit, inject } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Navmenu } from '../../shared/components/navmenu/navmenu';
import { Topbar } from '../../shared/components/topbar/topbar';
import { AppointmentService } from '../../core/services/appointment/appointment.service';
import { CustomerService } from '../../core/services/customer/customer.service';
import { VehicleService, Vehicle } from '../../core/services/vehicle/vehicle.service';
import { Appointment, NewAppointment } from '../../core/models/appointment.model';
import { Customer } from '../../core/models/customer.model';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';

@Component({
  selector: 'app-appointment-management',
  standalone: true,
  imports: [Navmenu, Topbar, FormsModule, CommonModule, DatePipe],
  providers: [DatePipe],
  templateUrl: './appointment-management.html',
  styleUrl: './appointment-management.css',
})
export class AppointmentManagement implements OnInit {
  private appointmentService = inject(AppointmentService);
  private customerService = inject(CustomerService);
  private vehicleService = inject(VehicleService);

  appointments: Appointment[] = [];
  filteredAppointments: Appointment[] = [];
  customers: Customer[] = [];
  vehicles: Vehicle[] = [];
  
  searchQuery: string = '';
  statusFilter: string = '';
  startDateFilter: string = '';
  endDateFilter: string = '';
  private searchSubject = new Subject<string>();
  
  showAddDialog: boolean = false;
  showEditDialog: boolean = false;
  isLoading: boolean = false;

  newAppointment: any = {
    customerId: '',
    vehicleId: '',
    date: '',
    time: '',
    status: 'Pending',
    notes: ''
  };

  selectedAppointment: any = null;

  ngOnInit() {
    this.loadAppointments();
    this.loadCustomers();
    this.loadVehicles();

    // Setup search debounce
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(() => {
      this.applyFilter();
    });
  }

  loadAppointments() {
    this.isLoading = true;
    this.appointmentService.getAll().subscribe({
      next: (data) => {
        this.appointments = data;
        this.applyFilter();
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading appointments', err);
        this.isLoading = false;
      }
    });
  }

  loadCustomers() {
    this.customerService.getAll().subscribe(data => this.customers = data);
  }

  loadVehicles() {
    this.vehicleService.getAll().subscribe(data => this.vehicles = data);
  }

  onSearchChange(query: string) {
    this.searchSubject.next(query);
  }

  onFilterChange() {
    this.applyFilter();
  }

  applyFilter() {
    const query = this.searchQuery.toLowerCase().trim();
    this.filteredAppointments = this.appointments.filter((a) => {
      const customerName = this.getCustomerName(a.customerId).toLowerCase();
      const vehicleName = this.getVehicleName(a.vehicleId).toLowerCase();
      
      const matchesSearch =
        !query ||
        customerName.includes(query) ||
        vehicleName.includes(query) ||
        a.id.toLowerCase().includes(query);

      const matchesStatus =
        !this.statusFilter || a.status === this.statusFilter;

      let matchesDateRange = true;
      if (this.startDateFilter || this.endDateFilter) {
        const aptDate = new Date(a.appointmentDate);
        aptDate.setHours(0,0,0,0);

        if (this.startDateFilter) {
          const start = new Date(this.startDateFilter);
          start.setHours(0,0,0,0);
          if (aptDate < start) matchesDateRange = false;
        }

        if (this.endDateFilter) {
          const end = new Date(this.endDateFilter);
          end.setHours(0,0,0,0);
          if (aptDate > end) matchesDateRange = false;
        }
      }

      return matchesSearch && matchesStatus && matchesDateRange;
    });
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'Confirmed': return 'bg-blue-50 text-blue-700';
      case 'Pending': return 'bg-yellow-50 text-yellow-700';
      case 'Completed': return 'bg-green-50 text-green-700';
      case 'Cancelled': return 'bg-red-50 text-red-700';
      default: return 'bg-gray-50 text-gray-700';
    }
  }

  getCustomerName(id: string): string {
    const customer = this.customers.find(c => c.userId === id);
    return customer ? `${customer.firstName} ${customer.lastName}` : 'Unknown Customer';
  }

  getVehicleName(id: string): string {
    const vehicle = this.vehicles.find(v => v.id === id);
    return vehicle ? `${vehicle.make} ${vehicle.model} (${vehicle.licensePlate})` : 'Unknown Vehicle';
  }

  openAddDialog() {
    this.showAddDialog = true;
  }

  closeAddDialog() {
    this.showAddDialog = false;
    this.resetForm();
  }

  openEditDialog(apt: Appointment) {
    const dateObj = new Date(apt.appointmentDate);
    this.selectedAppointment = {
      ...apt,
      date: dateObj.toISOString().split('T')[0],
      time: dateObj.toTimeString().split(' ')[0].substring(0, 5)
    };
    this.showEditDialog = true;
  }

  closeEditDialog() {
    this.showEditDialog = false;
    this.selectedAppointment = null;
  }

  resetForm() {
    this.newAppointment = {
      customerId: '',
      vehicleId: '',
      date: '',
      time: '',
      status: 'Pending',
      notes: ''
    };
  }

  bookAppointment() {
    if (!this.newAppointment.customerId || !this.newAppointment.vehicleId || !this.newAppointment.date) return;

    const appointmentDate = new Date(`${this.newAppointment.date}T${this.newAppointment.time || '00:00'}:00`);
    
    const dto: NewAppointment = {
      customerId: this.newAppointment.customerId,
      vehicleId: this.newAppointment.vehicleId,
      appointmentDate: appointmentDate.toISOString(),
      status: this.newAppointment.status,
      notes: this.newAppointment.notes,
      createdDate: new Date().toISOString()
    };

    this.appointmentService.add(dto).subscribe({
      next: () => {
        this.loadAppointments();
        this.closeAddDialog();
      },
      error: (err) => console.error('Error booking appointment', err)
    });
  }

  updateAppointment() {
    if (!this.selectedAppointment) return;

    const appointmentDate = new Date(`${this.selectedAppointment.date}T${this.selectedAppointment.time || '00:00'}:00`);
    
    const dto: Appointment = {
      ...this.selectedAppointment,
      appointmentDate: appointmentDate.toISOString()
    };

    this.appointmentService.update(dto.id, dto).subscribe({
      next: () => {
        this.loadAppointments();
        this.closeEditDialog();
      },
      error: (err) => console.error('Error updating appointment', err)
    });
  }

  deleteAppointment(id: string) {
    if (confirm('Are you sure you want to cancel and delete this appointment?')) {
      this.appointmentService.delete(id).subscribe({
        next: () => {
          this.appointments = this.appointments.filter(a => a.id !== id);
          this.applyFilter();
        },
        error: (err) => console.error('Error deleting appointment', err)
      });
    }
  }
}
