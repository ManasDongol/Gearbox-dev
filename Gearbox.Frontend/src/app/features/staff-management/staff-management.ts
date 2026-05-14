import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Navmenu } from '../../shared/components/navmenu/navmenu';
import { Topbar } from '../../shared/components/topbar/topbar';
import { StaffService } from '../../core/services/staff/staff.service';
import { Staff, NewStaff } from '../../core/models/staff.model';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { ToastService } from '../../shared/components/toast/toast.service';
import { Spinner } from '../../shared/components/spinner/spinner';

@Component({
  selector: 'app-staff-management',
  standalone: true,
  imports: [Navmenu, Topbar, FormsModule, CommonModule, Spinner],
  templateUrl: './staff-management.html',
  styleUrl: './staff-management.css',
})
export class StaffManagement implements OnInit {
  private staffService = inject(StaffService);
  private toast = inject(ToastService);

  staff: Staff[] = [];
  filteredStaff: Staff[] = [];
  
  searchQuery: string = '';
  departmentFilter: string = '';
  private searchSubject = new Subject<string>();
  
  showRegisterDialog: boolean = false;
  showEditDialog: boolean = false;
  isLoading: boolean = false;

  newStaff: NewStaff = {
    userName: '',
    firstName: '',
    lastName: '',
    phoneNumber: '',
    address: '',
    email: '',
    password: '',
    department: '',
    jobTitle: ''
  };

  selectedStaff: Staff | null = null;

  ngOnInit() {
    this.loadStaff();

    // Setup search debounce
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(query => {
      this.applyFilter();
    });
  }

  loadStaff() {
    this.isLoading = true;
    this.staffService.getAll().subscribe({
      next: (data) => {
        this.staff = data;
        this.applyFilter();
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading staff', err);
        this.toast.error('Unable to load staff', 'Please try again.');
        this.isLoading = false;
      }
    });
  }

  onSearchChange(query: string) {
    this.searchSubject.next(query);
  }

  onFilterChange() {
    this.applyFilter();
  }

  applyFilter() {
    const query = this.searchQuery.toLowerCase().trim();
    this.filteredStaff = this.staff.filter((s) => {
      const matchesSearch =
        !query ||
        s.firstName.toLowerCase().includes(query) ||
        s.lastName.toLowerCase().includes(query) ||
        s.userId.toLowerCase().includes(query) ||
        s.department.toLowerCase().includes(query) ||
        s.role.toLowerCase().includes(query);

      const matchesDept =
        !this.departmentFilter || s.department === this.departmentFilter;

      return matchesSearch && matchesDept;
    });
  }

  openRegisterDialog() {
    this.showRegisterDialog = true;
  }

  closeRegisterDialog() {
    this.showRegisterDialog = false;
    this.resetForm();
  }

  openEditDialog(staff: Staff) {
    this.selectedStaff = { ...staff };
    this.showEditDialog = true;
  }

  closeEditDialog() {
    this.showEditDialog = false;
    this.selectedStaff = null;
  }

  resetForm() {
    this.newStaff = {
      userName: '',
      firstName: '',
      lastName: '',
      phoneNumber: '',
      address: '',
      email: '',
      password: '',
      department: '',
      jobTitle: ''
    };
  }

  registerStaff() {
    if (!this.newStaff.userName || !this.newStaff.email || !this.newStaff.password) return;

    this.isLoading = true;
    this.staffService.add(this.newStaff).subscribe({
      next: () => {
        this.loadStaff();
        this.closeRegisterDialog();
        this.toast.success('Staff member added', 'The staff account was created.');
      },
      error: (err) => {
        console.error('Error registering staff', err);
        this.isLoading = false;
        this.toast.error('Unable to add staff member', 'Please check the details and try again.');
      }
    });
  }

  updateStaff() {
    if (!this.selectedStaff) return;

    this.isLoading = true;
    this.staffService.update(this.selectedStaff.userId, this.selectedStaff).subscribe({
      next: () => {
        const index = this.staff.findIndex(s => s.userId === this.selectedStaff?.userId);
        if (index !== -1 && this.selectedStaff) {
          this.staff[index] = { ...this.selectedStaff };
          this.applyFilter();
        }
        this.closeEditDialog();
        this.isLoading = false;
        this.toast.success('Staff member updated', 'The staff changes were saved.');
      },
      error: (err) => {
        console.error('Error updating staff', err);
        this.isLoading = false;
        this.toast.error('Unable to update staff member', 'Please try again.');
      }
    });
  }

  deleteStaff(id: string) {
    const staffMember = this.staff.find(s => s.userId === id);
    if (staffMember?.role === 'Admin') {
      this.toast.error('Admins cannot be deleted', 'Demote or review the admin account separately.');
      return;
    }

    if (confirm('Are you sure you want to delete this staff member?')) {
      this.isLoading = true;
      this.staffService.delete(id).subscribe({
        next: () => {
          this.staff = this.staff.filter(s => s.userId !== id);
          this.applyFilter();
          this.isLoading = false;
          this.toast.success('Staff member deleted', 'The staff member was removed.');
        },
        error: (err) => {
          console.error('Error deleting staff', err);
          this.isLoading = false;
          this.toast.error('Unable to delete staff member', 'Please try again.');
        }
      });
    }
  }

  promoteToAdmin(staff: Staff) {
    if (staff.role === 'Admin') return;

    if (confirm(`Promote ${staff.firstName} ${staff.lastName} to Admin?`)) {
      this.isLoading = true;
      this.staffService.promoteToAdmin(staff.userId).subscribe({
        next: () => {
          staff.role = 'Admin';
          this.applyFilter();
          this.isLoading = false;
          this.toast.success('Staff promoted', 'The account now has admin access.');
        },
        error: (err) => {
          console.error('Error promoting staff', err);
          this.isLoading = false;
          this.toast.error('Unable to promote staff member', 'Please try again.');
        }
      });
    }
  }
}
