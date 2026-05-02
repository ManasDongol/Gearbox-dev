import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Navmenu } from '../../shared/components/navmenu/navmenu';
import { PartService } from '../../core/services/parts/part.service';
import { VendorService } from '../../core/services/vendor/vendor';
import { Part, NewPart } from '../../core/Models/part.model';
import { Vendor } from '../../core/Models/vendor.model';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';

@Component({
  selector: 'app-inventory',
  standalone: true,
  imports: [Navmenu, FormsModule, CommonModule],
  templateUrl: './inventory.html',
  styleUrl: './inventory.css',
})
export class Inventory implements OnInit {
  private partService = inject(PartService);
  private vendorService = inject(VendorService);

  parts: Part[] = [];
  filteredParts: Part[] = [];
  vendors: Vendor[] = [];
  
  searchQuery: string = '';
  private searchSubject = new Subject<string>();
  
  showAddDialog: boolean = false;
  isLoading: boolean = false;

  newPart: NewPart = {
    name: '',
    description: '',
    partNumber: '',
    sellingPrice: 0,
    stockQuantity: 0,
    vendorId: ''
  };

  ngOnInit() {
    this.loadParts();
    this.loadVendors();

    // Setup search debounce
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(query => {
      this.applyFilter(query);
    });
  }

  loadParts() {
    this.isLoading = true;
    this.partService.getAll().subscribe({
      next: (data) => {
        this.parts = data;
        this.applyFilter(this.searchQuery);
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading parts', err);
        this.isLoading = false;
      }
    });
  }

  loadVendors() {
    this.vendorService.getAll().subscribe({
      next: (data) => {
        this.vendors = data;
      },
      error: (err) => {
        console.error('Error loading vendors', err);
      }
    });
  }

  onSearchChange(query: string) {
    this.searchSubject.next(query);
  }

  applyFilter(query: string) {
    if (!query.trim()) {
      this.filteredParts = this.parts;
    } else {
      const q = query.toLowerCase();
      this.filteredParts = this.parts.filter(p => 
        p.name.toLowerCase().includes(q) || 
        p.partNumber.toLowerCase().includes(q) ||
        p.description?.toLowerCase().includes(q)
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
    this.newPart = {
      name: '',
      description: '',
      partNumber: '',
      sellingPrice: 0,
      stockQuantity: 0,
      vendorId: ''
    };
  }

  addPart() {
    if (!this.newPart.name || !this.newPart.vendorId) return;

    this.partService.add(this.newPart).subscribe({
      next: (part) => {
        this.parts.push(part);
        this.applyFilter(this.searchQuery);
        this.closeAddDialog();
      },
      error: (err) => {
        console.error('Error adding part', err);
      }
    });
  }

  getVendorName(vendorId: string): string {
    return this.vendors.find(v => v.id === vendorId)?.name || 'Unknown';
  }
}
