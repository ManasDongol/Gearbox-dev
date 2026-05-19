import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Navmenu } from '../../shared/components/navmenu/navmenu';
import { Topbar } from '../../shared/components/topbar/topbar';
import { PartService } from '../../core/services/parts/part.service';
import { VendorService } from '../../core/services/vendor/vendor';
import { Part, NewPart } from '../../core/models/part.model';
import { Vendor } from '../../core/models/vendor.model';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { ToastService } from '../../shared/components/toast/toast.service';
import { Spinner } from '../../shared/components/spinner/spinner';
import { ConfirmCardService } from '../../shared/components/confirm-card/confirm-card.service';

@Component({
  selector: 'app-inventory',
  standalone: true,
  imports: [Navmenu, Topbar, FormsModule, CommonModule,Spinner],
  templateUrl: './inventory.html',
  styleUrl: './inventory.css',
})
export class Inventory implements OnInit {
  private partService = inject(PartService);
  private vendorService = inject(VendorService);
  private confirmCard = inject(ConfirmCardService);
  private toast = inject(ToastService);

  parts: Part[] = [];
  filteredParts: Part[] = [];
  vendors: Vendor[] = [];
  
  searchQuery: string = '';
  private searchSubject = new Subject<string>();
  
  showAddDialog: boolean = false;
  showEditDialog: boolean = false;
  isLoading: boolean = false;

  newPart: NewPart = {
    name: '',
    description: '',
    partNumber: '',
    sellingPrice: 0,
    stockQuantity: 0,
    vendorId: ''
  };

  selectedPart: Part | null = null;

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

  openEditDialog(part: Part) {
    this.selectedPart = { ...part };
    this.showEditDialog = true;
  }

  closeEditDialog() {
    this.showEditDialog = false;
    this.selectedPart = null;
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
        this.toast.success("part added successfully!","");
      },
      error: (err) => {
        console.error('Error adding part', err);
        this.toast.error("failed to add part! try again later!","");
      }
    });
  }

  updatePart() {
    if (!this.selectedPart) return;

    this.partService.update(this.selectedPart.id, this.selectedPart).subscribe({
      next: () => {
        const index = this.parts.findIndex(p => p.id === this.selectedPart?.id);
        if (index !== -1 && this.selectedPart) {
          this.parts[index] = { ...this.selectedPart };
          this.applyFilter(this.searchQuery);
        }
        this.closeEditDialog();
        this.toast.success("part updated successfully!","");
      },
      error: (err) => {
        console.error('Error updating part', err);
        this.toast.error("failed to update part!","");
      }
    });
  }

  async deletePart(id: string) {
    const confirmed = await this.confirmCard.confirm({
      title: 'Delete part?',
      message: 'Are you sure you want to delete this part from inventory?',
      confirmText: 'OK',
    });
    if (!confirmed) return;

    this.partService.delete(id).subscribe({
      next: () => {
        this.parts = this.parts.filter(p => p.id !== id);
        this.applyFilter(this.searchQuery);
        this.toast.success("part removed successfully!","");
      },
      error: (err) => {
        console.error('Error deleting part', err);
        this.toast.error("failed to remove part! please try again later","");
      }
    });
  }

  getVendorName(vendorId: string): string {
    return this.vendors.find(v => v.id === vendorId)?.name || 'Unknown';
  }
}
