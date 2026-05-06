import { Component, Input, inject, OnInit } from '@angular/core';
import { RouterLink, RouterLinkActive, Router } from '@angular/router';
import { Auth } from '../../../core/services/auth/auth';

@Component({
  selector: 'app-navmenu',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './navmenu.html',
  styleUrl: './navmenu.css',
})
export class Navmenu implements OnInit {
  @Input() activeItem: string = '';

  auth = inject(Auth);
  router = inject(Router);

  menuGroups: any[] = [];

  ngOnInit() {
    const isAdminOrStaff = this.auth.hasRole('Admin') || this.auth.hasRole('Staff');
    const isCustomer = this.auth.hasRole('Customer');

    if (isAdminOrStaff) {
      this.menuGroups = [
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
    } else if (isCustomer) {
      this.menuGroups = [
        {
          items: [
            { label: 'Dashboard', route: '/user-dashboard' },
            { label: 'My Appointments', route: '/my-appointments' },
          ],
        },
        {
          items: [
            { label: 'My Vehicles', route: '/my-vehicles' },
            { label: 'My Invoices', route: '/my-invoices' },
            { label: 'Requests', route: '/requests' },
            { label: 'Profile', route: '/profile' },
          ],
        },
      ];
    }
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }

  get homeRoute(): string {
    return this.auth.hasRole('Customer') ? '/user-dashboard' : '/dashboard';
  }
}
