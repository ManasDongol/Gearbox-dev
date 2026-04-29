import { Component, Input } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-navmenu',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './navmenu.html',
  styleUrl: './navmenu.css',
})
export class Navmenu {
  @Input() activeItem: string = '';

  menuGroups = [
    {
      items: [
        { label: 'Dashboard', route: '/dashboard' },
        { label: 'Staff Management', route: '/staff-management' },
        { label: 'Customer Management', route: '/customer-management' },
        { label: 'Appointments', route: '/appointment-management' },
      ],
    },
    {
      items: [
        { label: 'Inventory', route: '/inventory' },
        { label: 'Vendors', route: '/vendors' },
        { label: 'Purchase Invoices', route: '/purchase-invoices' },
      ],
    },
    {
      items: [
        { label: 'Sales Invoices', route: '/sales-invoices' },
        { label: 'Reports', route: '/reports' },
        { label: 'Notifications', route: '/notifications' },
        { label: 'Settings', route: '/settings' },
      ],
    },
  ];
}
