import { Component, OnInit, inject } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Navmenu } from '../../shared/components/navmenu/navmenu';
import { Topbar } from '../../shared/components/topbar/topbar';
import { SalesInvoiceService } from '../../core/services/sales-invoice/sales-invoice.service';
import { CustomerService } from '../../core/services/customer/customer.service';
import { StaffService } from '../../core/services/staff/staff.service';
import { PartService } from '../../core/services/parts/part.service';
import { ServiceService } from '../../core/services/service/service.service';
import { Vehicle, VehicleService } from '../../core/services/vehicle/vehicle.service';
import { SalesInvoice, NewSalesInvoice } from '../../core/models/sales-invoice.model';
import { Customer } from '../../core/models/customer.model';
import { Staff } from '../../core/models/staff.model';
import { Part } from '../../core/models/part.model';
import { Service } from '../../core/models/service.model';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { ToastService } from '../../shared/components/toast/toast.service';
import { Spinner } from '../../shared/components/spinner/spinner';

@Component({
  selector: 'app-sales-invoices',
  standalone: true,
  imports: [Navmenu, Topbar, FormsModule, CommonModule, DatePipe,Spinner],
  providers: [DatePipe],
  templateUrl: './sales-invoices.html',
  styleUrl: './sales-invoices.css',
})
export class SalesInvoices implements OnInit {
  private invoiceService = inject(SalesInvoiceService);
  private customerService = inject(CustomerService);
  private staffService = inject(StaffService);
  private partService = inject(PartService);
  private serviceService = inject(ServiceService);
  private vehicleService = inject(VehicleService);
  private toast = inject(ToastService);

  invoices: SalesInvoice[] = [];
  filteredInvoices: SalesInvoice[] = [];
  customers: Customer[] = [];
  staff: Staff[] = [];
  parts: Part[] = [];
  services: Service[] = [];
  customerVehicles: Vehicle[] = [];
  
  searchQuery: string = '';
  private searchSubject = new Subject<string>();
  
  showAddDialog: boolean = false;
  isLoading: boolean = false;
  isLoadingInvoiceItems: boolean = false;
  selectedInvoice: SalesInvoice | null = null;

  newInvoice: any = {
    customerId: '',
    staffId: '',
    invoiceNumber: '',
    totalAmount: 0,
    discountAmount: 0,
    items: [],
    serviceItems: []
  };

  ngOnInit() {
    this.loadInvoices();
    this.loadCustomers();
    this.loadStaff();
    this.loadParts();
    this.loadServices();

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

  loadServices() {
    this.serviceService.getAll().subscribe(data => this.services = data);
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
    this.addPartItem();
  }

  closeAddDialog() {
    this.showAddDialog = false;
  }

  onCustomerChange() {
    this.customerVehicles = [];
    this.newInvoice.serviceItems = this.newInvoice.serviceItems.map((item: any) => ({
      ...item,
      vehicleId: ''
    }));

    if (!this.newInvoice.customerId) return;

    this.vehicleService.getByCustomerId(this.newInvoice.customerId).subscribe({
      next: (vehicles) => {
        this.customerVehicles = vehicles;
      },
      error: (err) => {
        console.error('Error loading customer vehicles', err);
        this.customerVehicles = [];
      }
    });
  }

  viewInvoice(invoice: SalesInvoice) {
    this.selectedInvoice = invoice;
    this.isLoadingInvoiceItems = true;

    this.invoiceService.getById(invoice.id).subscribe({
      next: (invoiceDetails) => {
        this.selectedInvoice = invoiceDetails;
        this.isLoadingInvoiceItems = false;
      },
      error: (err) => {
        console.error('Error loading sales invoice details', err);
        this.isLoadingInvoiceItems = false;
      }
    });
  }

  closeInvoiceDetails() {
    this.selectedInvoice = null;
    this.isLoadingInvoiceItems = false;
  }

  resetForm() {
    this.newInvoice = {
      customerId: '',
      staffId: '',
      totalAmount: 0,
      discountAmount: 0,
      items: [],
      serviceItems: []
    };
    this.customerVehicles = [];
  }

  addPartItem() {
    this.newInvoice.items.push({
      partId: '',
      type: 'Part',
      quantity: 1,
      unitPrice: 0
    });
  }

  addServiceItem() {
    this.newInvoice.serviceItems.push({
      serviceId: '',
      vehicleId: '',
      type: 'Service',
      quantity: 1,
      unitPrice: 0
    });
  }

  removePartItem(index: number) {
    this.newInvoice.items.splice(index, 1);
    this.calculateTotal();
  }

  removeServiceItem(index: number) {
    this.newInvoice.serviceItems.splice(index, 1);
    this.calculateTotal();
  }

  onPartChange(item: any) {
    const part = this.parts.find(p => p.id === item.partId);
    if (part) {
      item.unitPrice = part.sellingPrice;
      this.calculateTotal();
    }
  }

  onServiceChange(item: any) {
    const service = this.services.find(s => s.id === item.serviceId);
    if (service) {
      item.unitPrice = service.price;
      this.calculateTotal();
    }
  }

  calculateTotal() {
    const partSubtotal = this.newInvoice.items.reduce((acc: number, item: any) => acc + (item.quantity * item.unitPrice), 0);
    const serviceSubtotal = this.newInvoice.serviceItems.reduce((acc: number, item: any) => acc + (item.quantity * item.unitPrice), 0);
    const subtotal = partSubtotal + serviceSubtotal;
    this.newInvoice.totalAmount = subtotal - this.newInvoice.discountAmount;
  }

  saveInvoice() {
    const serviceMissingVehicle = this.newInvoice.serviceItems.some((i: any) => i.serviceId && !i.vehicleId);
    if (serviceMissingVehicle) {
      this.toast.error('Vehicle required', 'Select a customer vehicle for every service item.');
      return;
    }

    const invoiceItems = [
      ...this.newInvoice.items
        .filter((i: any) => i.partId)
        .map((i: any) => ({
          partId: i.partId,
          serviceId: undefined,
          type: 'Part',
          quantity: i.quantity,
          unitPrice: i.unitPrice
        })),
      ...this.newInvoice.serviceItems
        .filter((i: any) => i.serviceId)
        .map((i: any) => ({
          partId: undefined,
          serviceId: i.serviceId,
          vehicleId: i.vehicleId,
          type: 'Service',
          quantity: i.quantity,
          unitPrice: i.unitPrice
        }))
    ];

    if (!this.newInvoice.customerId || !this.newInvoice.staffId || invoiceItems.length === 0) return;

    const dto: NewSalesInvoice = {
      customerId: this.newInvoice.customerId,
      staffId: this.newInvoice.staffId,
      totalAmount: this.newInvoice.totalAmount,
      discountAmount: this.newInvoice.discountAmount,
      items: invoiceItems
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

  itemTotal(item: SalesInvoice['items'][number]): number {
    return item.quantity * item.unitPrice;
  }
}
