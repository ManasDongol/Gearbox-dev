import { Component, OnInit, inject } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Navmenu } from '../../shared/components/navmenu/navmenu';
import { Topbar } from '../../shared/components/topbar/topbar';
import { PurchaseInvoiceService } from '../../core/services/purchase-invoice/purchase-invoice.service';
import { VendorService } from '../../core/services/vendor/vendor';
import { PartService } from '../../core/services/parts/part.service';
import { PurchaseInvoice, NewPurchaseInvoice, NewPurchaseInvoiceItem } from '../../core/models/purchase-invoice.model';
import { Vendor } from '../../core/models/vendor.model';
import { Part } from '../../core/models/part.model';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { ToastService } from '../../shared/components/toast/toast.service';
import { Spinner } from '../../shared/components/spinner/spinner';
import { ConfirmCardService } from '../../shared/components/confirm-card/confirm-card.service';

@Component({
  selector: 'app-purchase-invoices',
  standalone: true,
  imports: [Navmenu, Topbar, FormsModule, CommonModule, DatePipe,Spinner],
  providers: [DatePipe],
  templateUrl: './purchase-invoices.html',
  styleUrl: './purchase-invoices.css',
})
export class PurchaseInvoices implements OnInit {
  private invoiceService = inject(PurchaseInvoiceService);
  private vendorService = inject(VendorService);
  private partService = inject(PartService);
  private confirmCard = inject(ConfirmCardService);

  invoices: PurchaseInvoice[] = [];
  filteredInvoices: PurchaseInvoice[] = [];
  vendors: Vendor[] = [];
  parts: Part[] = [];
  
  searchQuery: string = '';
  private searchSubject = new Subject<string>();
  
  showAddDialog: boolean = false;
  isLoading: boolean = false;

  newInvoice: any = {
    vendorId: '',
    invoiceNumber: '',
    totalAmount: 0,
    items: []
  };

  ngOnInit() {
    this.loadInvoices();
    this.loadVendors();
    this.loadParts();

    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(() => this.applyFilter());
  }

  loadInvoices() {
    this.isLoading = true;
    this.invoiceService.getAll().subscribe({
      next: (data) => {
        this.invoices = data;
        this.applyFilter();
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading invoices', err);
        this.isLoading = false;
      }
    });
  }

  loadVendors() {
    this.vendorService.getAll().subscribe(data => this.vendors = data);
  }

  loadParts() {
    this.partService.getAll().subscribe(data => this.parts = data);
  }

  onSearchChange(query: string) {
    this.searchSubject.next(query);
  }

  applyFilter() {
    const q = this.searchQuery.toLowerCase().trim();
    this.filteredInvoices = this.invoices.filter(inv => 
      inv.invoiceNumber.toLowerCase().includes(q) ||
      this.getVendorName(inv.vendorId).toLowerCase().includes(q)
    );
  }

  getVendorName(id: string): string {
    return this.vendors.find(v => v.id === id)?.name || 'Unknown Vendor';
  }

  getPartName(id: string): string {
    return this.parts.find(p => p.id === id)?.name || 'Unknown Part';
  }

  openAddDialog() {
    this.showAddDialog = true;
    this.resetForm();
    this.addItem(); // Start with one item
  }

  closeAddDialog() {
    this.showAddDialog = false;
  }

  resetForm() {
    this.newInvoice = {
      vendorId: '',
      invoiceNumber: '',
      totalAmount: 0,
      items: []
    };
  }

  addItem() {
    this.newInvoice.items.push({
      partId: '',
      quantity: 1,
      costPrice: 0
    });
  }

  removeItem(index: number) {
    this.newInvoice.items.splice(index, 1);
    this.calculateTotal();
  }

  calculateTotal() {
    this.newInvoice.totalAmount = this.newInvoice.items.reduce((acc: number, item: any) => acc + (item.quantity * item.costPrice), 0);
  }

  saveInvoice() {
    if (!this.newInvoice.vendorId || !this.newInvoice.invoiceNumber || this.newInvoice.items.length === 0) return;

    const dto: NewPurchaseInvoice = {
      vendorId: this.newInvoice.vendorId,
      invoiceNumber: this.newInvoice.invoiceNumber,
      totalAmount: this.newInvoice.totalAmount,
      items: this.newInvoice.items.map((i: any) => ({
        partId: i.partId,
        quantity: i.quantity,
        costPrice: i.costPrice
      }))
    };

    this.invoiceService.add(dto).subscribe({
      next: () => {
        this.loadInvoices();
        this.closeAddDialog();
      },
      error: (err) => console.error('Error saving invoice', err)
    });
  }

  async deleteInvoice(id: string) {
    const confirmed = await this.confirmCard.confirm({
      title: 'Delete invoice?',
      message: 'Are you sure you want to delete this invoice?',
      confirmText: 'OK',
    });
    if (!confirmed) return;

    this.invoiceService.delete(id).subscribe({
      next: () => {
        this.invoices = this.invoices.filter(i => i.id !== id);
        this.applyFilter();
      },
      error: (err) => console.error('Error deleting invoice', err)
    });
  }
}
