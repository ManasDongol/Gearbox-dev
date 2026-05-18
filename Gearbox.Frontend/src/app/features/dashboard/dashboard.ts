import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Navmenu } from '../../shared/components/navmenu/navmenu';
import { Topbar } from '../../shared/components/topbar/topbar';
import { ToastService } from '../../shared/components/toast/toast.service';
import { Spinner } from '../../shared/components/spinner/spinner';
import { PdfService } from '../../core/services/pdf/pdf.service';
import { SalesInvoiceService } from '../../core/services/sales-invoice/sales-invoice.service';
import { CustomerService } from '../../core/services/customer/customer.service';
import { AppointmentService } from '../../core/services/appointment/appointment.service';
import { PartService } from '../../core/services/parts/part.service';
import { ServiceHistory, ServiceHistoryService } from '../../core/services/service-history/service-history.service';
import { SalesInvoice } from '../../core/models/sales-invoice.model';
import { Customer } from '../../core/models/customer.model';
import { Appointment } from '../../core/models/appointment.model';
import { Part } from '../../core/models/part.model';

type DashboardStat = {
  title: string;
  value: string;
  trend: string;
  isPositive: boolean;
};

type RecentActivity = {
  id: string;
  action: string;
  details: string;
  time: string;
  type: 'invoice' | 'user' | 'service' | 'alert';
  date: Date;
};

type RevenueBar = {
  label: string;
  value: number;
  height: number;
  isPeak: boolean;
};

@Component({
  selector: 'app-dashboard',
  imports: [Navmenu, Topbar, CommonModule,Spinner],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard implements OnInit {
  private pdfService = inject(PdfService);
  private toast = inject(ToastService);
  private salesInvoiceService = inject(SalesInvoiceService);
  private customerService = inject(CustomerService);
  private appointmentService = inject(AppointmentService);
  private partService = inject(PartService);
  private serviceHistoryService = inject(ServiceHistoryService);

  isGeneratingFinancialReport = false;
  isGeneratingCustomerReport = false;
  isLoadingDashboard = true;

  stats: DashboardStat[] = [
    { title: 'Total Revenue', value: 'Rs. 0', trend: 'Live', isPositive: true },
    { title: 'Customers', value: '0', trend: 'Live', isPositive: true },
    { title: 'Pending Services', value: '0', trend: 'Live', isPositive: false },
    { title: 'Inventory Alerts', value: '0', trend: 'Live', isPositive: false },
  ];

  recentActivities: RecentActivity[] = [];
  revenueBars: RevenueBar[] = [
    { label: 'Mon', value: 0, height: 8, isPeak: false },
    { label: 'Tue', value: 0, height: 8, isPeak: false },
    { label: 'Wed', value: 0, height: 8, isPeak: false },
    { label: 'Thu', value: 0, height: 8, isPeak: false },
    { label: 'Fri', value: 0, height: 8, isPeak: false },
    { label: 'Sat', value: 0, height: 8, isPeak: false },
    { label: 'Sun', value: 0, height: 8, isPeak: false },
  ];

  ngOnInit() {
    this.loadDashboardData();
  }

  loadDashboardData() {
    this.isLoadingDashboard = true;

    forkJoin({
      invoices: this.salesInvoiceService.getAll().pipe(catchError(() => of([] as SalesInvoice[]))),
      customers: this.customerService.getAll().pipe(catchError(() => of([] as Customer[]))),
      appointments: this.appointmentService.getAll().pipe(catchError(() => of([] as Appointment[]))),
      parts: this.partService.getAll().pipe(catchError(() => of([] as Part[]))),
      serviceHistory: this.serviceHistoryService.getAll().pipe(catchError(() => of([] as ServiceHistory[]))),
    }).subscribe({
      next: ({ invoices, customers, appointments, parts, serviceHistory }) => {
        const totalRevenue =
          invoices.reduce((sum, invoice) => sum + invoice.totalAmount, 0) +
          serviceHistory.reduce((sum, history) => sum + history.totalCost, 0);
        const pendingServices = appointments.filter(
          (appointment) => !['Completed', 'Cancelled'].includes(appointment.status),
        ).length;
        const inventoryAlerts = parts.filter((part) => part.stockQuantity <= 5).length;

        this.stats = [
          { title: 'Total Revenue', value: this.formatCurrency(totalRevenue), trend: `${invoices.length} invoices`, isPositive: true },
          { title: 'Customers', value: customers.length.toLocaleString(), trend: 'Registered', isPositive: true },
          { title: 'Pending Services', value: pendingServices.toString(), trend: 'Open', isPositive: pendingServices === 0 },
          { title: 'Inventory Alerts', value: inventoryAlerts.toString(), trend: 'Low stock', isPositive: inventoryAlerts === 0 },
        ];

        this.revenueBars = this.buildRevenueBars(invoices);
        this.recentActivities = this.buildRecentActivities(invoices, customers, appointments, parts);
        this.isLoadingDashboard = false;
      },
      error: (err) => {
        console.error('Error loading dashboard data', err);
        this.toast.error('Dashboard failed', 'Could not load dashboard data.');
        this.isLoadingDashboard = false;
      },
    });
  }

  generateFinancialReport() {
    if (this.isGeneratingFinancialReport) return;

    this.isGeneratingFinancialReport = true;
    this.pdfService.generateFinancialReport().subscribe({
      next: (pdf) => {
        this.downloadPdf(pdf, this.createFileName('financial-report'));
        this.toast.success('Report ready', 'Financial report downloaded.');
        this.isGeneratingFinancialReport = false;
      },
      error: (err) => {
        console.error('Error generating financial report', err);
        this.toast.error('Report failed', 'Could not generate the financial report.');
        this.isGeneratingFinancialReport = false;
      }
    });
  }

  generateCustomerReport() {
    if (this.isGeneratingCustomerReport) return;

    this.isGeneratingCustomerReport = true;
    this.pdfService.generateCustomerReport().subscribe({
      next: (pdf) => {
        this.downloadPdf(pdf, this.createFileName('customer-report'));
        this.toast.success('Report ready', 'Customer report downloaded.');
        this.isGeneratingCustomerReport = false;
      },
      error: (err) => {
        console.error('Error generating customer report', err);
        this.toast.error('Report failed', 'Could not generate the customer report.');
        this.isGeneratingCustomerReport = false;
      }
    });
  }

  private downloadPdf(pdf: Blob, fileName: string) {
    const url = URL.createObjectURL(pdf);
    const link = document.createElement('a');
    link.href = url;
    link.download = fileName;
    link.click();
    URL.revokeObjectURL(url);
  }

  private createFileName(reportName: string): string {
    const date = new Date().toISOString().slice(0, 10);
    return `gearbox-${reportName}-${date}.pdf`;
  }

  private buildRevenueBars(invoices: SalesInvoice[]): RevenueBar[] {
    const labels = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];
    const totals = new Array(7).fill(0) as number[];
    const today = new Date();
    const dayOffset = (today.getDay() + 6) % 7;
    const weekStart = new Date(today);
    weekStart.setHours(0, 0, 0, 0);
    weekStart.setDate(today.getDate() - dayOffset);

    invoices.forEach((invoice) => {
      const createdAt = new Date(invoice.createdAt);
      if (Number.isNaN(createdAt.getTime()) || createdAt < weekStart) return;

      const index = (createdAt.getDay() + 6) % 7;
      totals[index] += invoice.totalAmount;
    });

    const max = Math.max(...totals, 0);

    return labels.map((label, index) => ({
      label,
      value: totals[index],
      height: max > 0 ? Math.max(8, Math.round((totals[index] / max) * 100)) : 8,
      isPeak: max > 0 && totals[index] === max,
    }));
  }

  private buildRecentActivities(
    invoices: SalesInvoice[],
    customers: Customer[],
    appointments: Appointment[],
    parts: Part[],
  ): RecentActivity[] {
    const customerNames = new Map(
      customers.map((customer) => [
        customer.userId,
        `${customer.firstName ?? ''} ${customer.lastName ?? ''}`.trim() || customer.userName,
      ]),
    );

    const invoiceActivities = invoices.map((invoice) => ({
      id: `invoice-${invoice.id}`,
      action: invoice.paymentStatus ? 'Invoice Paid' : 'Invoice Generated',
      details: `${this.formatCurrency(invoice.totalAmount)} for ${customerNames.get(invoice.customerId) ?? 'customer'}`,
      time: this.timeAgo(invoice.createdAt),
      type: 'invoice' as const,
      date: new Date(invoice.createdAt),
    }));

    const customerActivities = customers.map((customer) => ({
      id: `customer-${customer.userId}`,
      action: 'Customer Registered',
      details: `${customer.firstName ?? ''} ${customer.lastName ?? ''}`.trim() || customer.userName,
      time: this.timeAgo(customer.registeredSince),
      type: 'user' as const,
      date: new Date(customer.registeredSince),
    }));

    const appointmentActivities = appointments.map((appointment) => ({
      id: `appointment-${appointment.id}`,
      action: `Service ${appointment.status}`,
      details: appointment.notes || 'Service appointment',
      time: this.timeAgo(appointment.createdDate || appointment.appointmentDate),
      type: 'service' as const,
      date: new Date(appointment.createdDate || appointment.appointmentDate),
    }));

    const alertActivities = parts
      .filter((part) => part.stockQuantity <= 5)
      .map((part) => ({
        id: `part-${part.id}`,
        action: 'Low Stock Alert',
        details: `${part.name} has ${part.stockQuantity} left`,
        time: 'Now',
        type: 'alert' as const,
        date: new Date(),
      }));

    return [...invoiceActivities, ...customerActivities, ...appointmentActivities, ...alertActivities]
      .filter((activity) => !Number.isNaN(activity.date.getTime()))
      .sort((a, b) => b.date.getTime() - a.date.getTime())
      .slice(0, 6);
  }

  private timeAgo(dateValue: string): string {
    const date = new Date(dateValue);
    const seconds = Math.max(0, Math.floor((Date.now() - date.getTime()) / 1000));

    if (Number.isNaN(seconds)) return 'Recently';
    if (seconds < 60) return 'Just now';

    const minutes = Math.floor(seconds / 60);
    if (minutes < 60) return `${minutes} min${minutes === 1 ? '' : 's'} ago`;

    const hours = Math.floor(minutes / 60);
    if (hours < 24) return `${hours} hour${hours === 1 ? '' : 's'} ago`;

    const days = Math.floor(hours / 24);
    if (days < 30) return `${days} day${days === 1 ? '' : 's'} ago`;

    const months = Math.floor(days / 30);
    return `${months} month${months === 1 ? '' : 's'} ago`;
  }

  private formatCurrency(value: number): string {
    return `Rs. ${value.toLocaleString(undefined, { maximumFractionDigits: 0 })}`;
  }
}
