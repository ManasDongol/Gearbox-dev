import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Navmenu } from '../../shared/components/navmenu/navmenu';
import { Topbar } from '../../shared/components/topbar/topbar';
import { VendorService } from '../../core/services/vendor/vendor';
import { Vendor, NewVendor } from '../../core/models/vendor.model';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';

@Component({
  selector: 'app-vendor-management',
  standalone: true,
  imports: [Navmenu, Topbar, FormsModule, CommonModule],
  templateUrl: './vendor-management.html',
  styleUrl: './vendor-management.css',
})
export class VendorManagement implements OnInit {
  private vendorService = inject(VendorService);

  vendors: Vendor[] = [];
  filteredVendors: Vendor[] = [];
  
  searchQuery: string = '';
  private searchSubject = new Subject<string>();
  
  showAddDialog: boolean = false;
  showEditDialog: boolean = false;
  isLoading: boolean = false;

  newVendor: NewVendor = {
    name: '',
    phoneNumber: '',
    email: '',
    address: ''
  };

  selectedVendor: Vendor | null = null;

  ngOnInit() {
    this.loadVendors();

    // Setup search debounce
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(query => {
      this.applyFilter(query);
    });
  }

  loadVendors() {
    this.isLoading = true;
    this.vendorService.getAll().subscribe({
      next: (data) => {
        this.vendors = data;
        this.applyFilter(this.searchQuery);
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading vendors', err);
        this.isLoading = false;
      }
    });
  }

  onSearchChange(query: string) {
    this.searchSubject.next(query);
  }

  applyFilter(query: string) {
    if (!query.trim()) {
      this.filteredVendors = this.vendors;
    } else {
      const q = query.toLowerCase();
      this.filteredVendors = this.vendors.filter(v => 
        v.name.toLowerCase().includes(q) || 
        v.email.toLowerCase().includes(q) || 
        v.phoneNumber.toLowerCase().includes(q) ||
        v.address.toLowerCase().includes(q)
      );
    }
  }

  openAddDialog() {
    this.showAddDialog = true;
  }

  closeAddDialog() {
    this.showAddDialog = false;
    this.resetForm();
  }

  openEditDialog(vendor: Vendor) {
    this.selectedVendor = { ...vendor };
    this.showEditDialog = true;
  }

  closeEditDialog() {
    this.showEditDialog = false;
    this.selectedVendor = null;
  }

  resetForm() {
    this.newVendor = {
      name: '',
      phoneNumber: '',
      email: '',
      address: ''
    };
  }

  addVendor() {
    if (!this.newVendor.name || !this.newVendor.email) return;

    this.vendorService.add(this.newVendor).subscribe({
      next: (vendor) => {
        this.vendors.push(vendor);
        this.applyFilter(this.searchQuery);
        this.closeAddDialog();
      },
      error: (err) => {
        console.error('Error adding vendor', err);
      }
    });
  }

  updateVendor() {
    if (!this.selectedVendor) return;

    this.vendorService.update(this.selectedVendor.id, this.selectedVendor).subscribe({
      next: () => {
        const index = this.vendors.findIndex(v => v.id === this.selectedVendor?.id);
        if (index !== -1 && this.selectedVendor) {
          this.vendors[index] = { ...this.selectedVendor };
          this.applyFilter(this.searchQuery);
        }
        this.closeEditDialog();
      },
      error: (err) => {
        console.error('Error updating vendor', err);
      }
    });
  }

  deleteVendor(id: string) {
    if (confirm('Are you sure you want to delete this vendor?')) {
      this.vendorService.delete(id).subscribe({
        next: () => {
          this.vendors = this.vendors.filter(v => v.id !== id);
          this.applyFilter(this.searchQuery);
        },
        error: (err) => {
          console.error('Error deleting vendor', err);
        }
      });
    }
  }
}
