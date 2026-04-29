import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { Navmenu } from '../../shared/components/navmenu/navmenu';

@Component({
  selector: 'app-appointment-management',
  imports: [Navmenu, FormsModule, DatePipe],
  providers: [DatePipe],
  templateUrl: './appointment-management.html',
  styleUrl: './appointment-management.css',
})
export class AppointmentManagement {
  appointments = [
    {
      id: 'APT-1001',
      customerName: 'Aarav Sharma',
      vehicle: 'Toyota Corolla 2018',
      appointmentDate: new Date('2026-05-01T10:00:00'),
      status: 'Confirmed',
      notes: 'Regular oil change and tire rotation',
    },
    {
      id: 'APT-1002',
      customerName: 'Priya Thapa',
      vehicle: 'Honda Civic 2020',
      appointmentDate: new Date('2026-05-02T14:30:00'),
      status: 'Pending',
      notes: 'Brake pad replacement check',
    },
    {
      id: 'APT-1003',
      customerName: 'Rohan Karki',
      vehicle: 'Ford Ranger 2019',
      appointmentDate: new Date('2026-04-28T09:15:00'),
      status: 'Completed',
      notes: 'Engine diagnostic',
    },
    {
      id: 'APT-1004',
      customerName: 'Sita Gurung',
      vehicle: 'Hyundai Creta 2022',
      appointmentDate: new Date('2026-05-05T11:00:00'),
      status: 'Cancelled',
      notes: 'Customer requested reschedule',
    },
    {
      id: 'APT-1005',
      customerName: 'Bikash Rai',
      vehicle: 'Suzuki Swift 2017',
      appointmentDate: new Date('2026-05-10T15:00:00'),
      status: 'Confirmed',
      notes: 'AC servicing',
    }
  ];

  searchQuery: string = '';
  statusFilter: string = '';
  startDateFilter: string = '';
  endDateFilter: string = '';
  showAddDialog: boolean = false;

  get filteredAppointments() {
    return this.appointments.filter((a) => {
      const matchesSearch =
        !this.searchQuery.trim() ||
        a.customerName.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
        a.vehicle.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
        a.id.toLowerCase().includes(this.searchQuery.toLowerCase());

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
      case 'Confirmed':
        return 'bg-blue-50 text-blue-700';
      case 'Pending':
        return 'bg-yellow-50 text-yellow-700';
      case 'Completed':
        return 'bg-green-50 text-green-700';
      case 'Cancelled':
        return 'bg-red-50 text-red-700';
      default:
        return 'bg-gray-50 text-gray-700';
    }
  }

  openAddDialog() {
    this.showAddDialog = true;
  }

  closeAddDialog() {
    this.showAddDialog = false;
  }
}
