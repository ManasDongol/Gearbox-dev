import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Navmenu } from '../../shared/components/navmenu/navmenu';
import { Topbar } from '../../shared/components/topbar/topbar';
import { ToastService } from '../../shared/components/toast/toast.service';
import { Spinner } from '../../shared/components/spinner/spinner';
import { PdfService } from '../../core/services/pdf/pdf.service';

@Component({
  selector: 'app-dashboard',
  imports: [Navmenu, Topbar, CommonModule,Spinner],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard {
  private pdfService = inject(PdfService);
  private toast = inject(ToastService);

  isGeneratingFinancialReport = false;
  isGeneratingCustomerReport = false;

  stats = [
    { title: 'Total Revenue', value: 'Rs. 4,50,000', trend: '+12.5%', isPositive: true },
    { title: 'Active Customers', value: '1,240', trend: '+5.2%', isPositive: true },
    { title: 'Pending Services', value: '34', trend: '-2.4%', isPositive: false },
    { title: 'Inventory Alerts', value: '12', trend: '+1.1%', isPositive: false },
  ];

  recentActivities = [
    { id: 1, action: 'Invoice Generated', details: 'INV-2024-001 for John Doe', time: '10 mins ago', type: 'invoice' },
    { id: 2, action: 'New Customer', details: 'Sarah Smith registered', time: '1 hour ago', type: 'user' },
    { id: 3, action: 'Service Completed', details: 'Oil change for Toyota Corolla', time: '2 hours ago', type: 'service' },
    { id: 4, action: 'Low Stock Alert', details: 'Brake pads running low', time: '4 hours ago', type: 'alert' },
  ];

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
}
