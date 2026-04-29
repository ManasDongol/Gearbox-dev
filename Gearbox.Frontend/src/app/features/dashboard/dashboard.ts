import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Navmenu } from '../../shared/components/navmenu/navmenu';

@Component({
  selector: 'app-dashboard',
  imports: [Navmenu, CommonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard {
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
    console.log('Generating financial report...');
    alert('Financial Report Generation Started.');
  }

  generateCustomerReport() {
    console.log('Generating customer report...');
    alert('Customer Report Generation Started.');
  }
}
