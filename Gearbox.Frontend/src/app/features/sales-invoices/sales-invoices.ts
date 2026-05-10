import { Component, OnInit, inject } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Navmenu } from '../../shared/components/navmenu/navmenu';
import { Topbar } from '../../shared/components/topbar/topbar';
import { SalesInvoiceService } from '../../core/services/sales-invoice/sales-invoice.service';
import { CustomerService } from '../../core/services/customer/customer.service';
import { StaffService } from '../../core/services/staff/staff.service';
import { PartService } from '../../core/services/parts/part.service';
import { SalesInvoice, NewSalesInvoice, SalesInvoiceItem } from '../../core/models/sales-invoice.model';
import { Customer } from '../../core/models/customer.model';
import { Staff } from '../../core/models/staff.model';
import { Part } from '../../core/models/part.model';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { ToastService } from '../../shared/components/toast/toast.service';
import { Spinner } from '../../shared/components/spinner/spinner';

@Component({
  selector: 'app-sales-invoices',
  standalone: true,
  imports: [Navmenu, Topbar, FormsModule, CommonModule, DatePipe],
  providers: [DatePipe],
  templateUrl: './sales-invoices.html',
  styleUrl: './sales-invoices.css',
})
export class SalesInvoices implements OnInit {
  private invoiceService = inject(SalesInvoiceService);
  private customerService = inject(CustomerService);
  private staffService = inject(StaffService);
  private partService = inject(PartService);

  invoices: SalesInvoice[] = [];
  filteredInvoices: SalesInvoice[] = [];
  customers: Customer[] = [];
  staff: Staff[] = [];
  parts: Part[] = [];
  
  searchQuery: string = '';
  private searchSubject = new Subject<string>();
  
  showAddDialog: boolean = false;
  isLoading: boolean = false;

  newInvoice: any = {
    customerId: '',
    staffId: '',
    invoiceNumber: '',
    totalAmount: 0,
    discountAmount: 0,
    items: []
  };

  ngOnInit() {
    this.loadInvoices();
    this.loadCustomers();
    this.loadStaff();
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

  loadCustomers() {
    this.customerService.getAll().subscribe(data => this.customers = data);
  }

  loadStaff() {
    this.staffService.getAll().subscribe(data => this.staff = data);
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
      inv.id.toLowerCase().includes(q) ||
      this.getCustomerName(inv.customerId).toLowerCase().includes(q)
    );
  }

  getCustomerName(id: string): string {
    const c = this.customers.find(c => c.userId === id);
    return c ? `${c.firstName} ${c.lastName}` : 'Unknown Customer';
  }

  getStaffName(id: string): string {
    const s = this.staff.find(s => s.userId === id);
    return s ? `${s.firstName} ${s.lastName}` : 'Unknown Staff';
  }

  openAddDialog() {
    this.showAddDialog = true;
    this.resetForm();
    this.addItem();
  }

  closeAddDialog() {
    this.showAddDialog = false;
  }

  resetForm() {
    this.newInvoice = {
      customerId: '',
      staffId: '',
      totalAmount: 0,
      discountAmount: 0,
      items: []
    };
  }

  addItem() {
    this.newInvoice.items.push({
      partId: '',
      type: 'Part',
      quantity: 1,
      unitPrice: 0
    });
  }

  removeItem(index: number) {
    this.newInvoice.items.splice(index, 1);
    this.calculateTotal();
  }

  onPartChange(item: any) {
    const part = this.parts.find(p => p.id === item.partId);
    if (part) {
      item.unitPrice = part.sellingPrice;
      this.calculateTotal();
    }
  }

  calculateTotal() {
    const subtotal = this.newInvoice.items.reduce((acc: number, item: any) => acc + (item.quantity * item.unitPrice), 0);
    this.newInvoice.totalAmount = subtotal - this.newInvoice.discountAmount;
  }

  saveInvoice() {
    if (!this.newInvoice.customerId || !this.newInvoice.staffId || this.newInvoice.items.length === 0) return;

    const dto: NewSalesInvoice = {
      customerId: this.newInvoice.customerId,
      staffId: this.newInvoice.staffId,
      totalAmount: this.newInvoice.totalAmount,
      discountAmount: this.newInvoice.discountAmount,
      items: this.newInvoice.items.map((i: any) => ({
        partId: i.partId,
        type: 'Part',
        quantity: i.quantity,
        unitPrice: i.unitPrice
      }))
    };

    this.invoiceService.add(dto).subscribe({
      next: () => {
        this.loadInvoices();
        this.closeAddDialog();
      },
      error: (err) => console.error('Error saving sales invoice', err)
    });
  }

  deleteInvoice(id: string) {
    if (confirm('Are you sure you want to delete this invoice?')) {
      this.invoiceService.delete(id).subscribe({
        next: () => {
          this.invoices = this.invoices.filter(i => i.id !== id);
          this.applyFilter();
        },
        error: (err) => console.error('Error deleting invoice', err)
      });
    }
  }
}
