import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Navmenu } from '../../shared/components/navmenu/navmenu';
import { VendorService } from '../../core/services/vendor/vendor';
import { Vendor, NewVendor } from '../../core/Models/vendor.model';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';

@Component({
  selector: 'app-vendor-management',
  standalone: true,
  imports: [Navmenu, FormsModule, CommonModule],
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
  isLoading: boolean = false;

  newVendor: NewVendor = {
    name: '',
    phoneNumber: '',
    email: '',
    address: ''
  };

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
}
