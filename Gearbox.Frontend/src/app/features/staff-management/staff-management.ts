import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Navmenu } from '../../shared/components/navmenu/navmenu';

@Component({
  selector: 'app-staff-management',
  imports: [Navmenu, FormsModule],
  templateUrl: './staff-management.html',
  styleUrl: './staff-management.css',
})
export class StaffManagement {
  staff = [
    {
      id: 'STF-001',
      name: 'Aarav Sharma',
      email: 'aarav.sharma@gearbox.com',
      department: 'Sales',
      status: 'Active',
    },
    {
      id: 'STF-002',
      name: 'Priya Thapa',
      email: 'priya.thapa@gearbox.com',
      department: 'Mechanics',
      status: 'Active',
    },
    {
      id: 'STF-003',
      name: 'Rohan Karki',
      email: 'rohan.karki@gearbox.com',
      department: 'Inventory',
      status: 'Inactive',
    },
    {
      id: 'STF-004',
      name: 'Sita Gurung',
      email: 'sita.gurung@gearbox.com',
      department: 'Admin',
      status: 'Active',
    },
    {
      id: 'STF-005',
      name: 'Bikash Rai',
      email: 'bikash.rai@gearbox.com',
      department: 'Sales',
      status: 'Active',
    },
  ];

  searchQuery: string = '';
  departmentFilter: string = '';
  statusFilter: string = '';
  showRegisterDialog: boolean = false;

  get filteredStaff() {
    return this.staff.filter((s) => {
      const matchesSearch =
        !this.searchQuery.trim() ||
        s.name.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
        s.email.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
        s.id.toLowerCase().includes(this.searchQuery.toLowerCase());

      const matchesDept =
        !this.departmentFilter || s.department === this.departmentFilter;

      const matchesStatus =
        !this.statusFilter || s.status === this.statusFilter;

      return matchesSearch && matchesDept && matchesStatus;
    });
  }

  openRegisterDialog() {
    this.showRegisterDialog = true;
  }

  closeRegisterDialog() {
    this.showRegisterDialog = false;
  }
}
