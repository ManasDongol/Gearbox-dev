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
    const isAdmin = this.auth.hasRole('Admin') ;
    const isStaff = this.auth.hasRole('Staff');
    const isCustomer = this.auth.hasRole('Customer');

    if (isAdmin) {
      this.menuGroups = [
        {
          items: [
            { label: 'Dashboard', route: '/dashboard' },
            { label: 'Staffs', route: '/staff-management' },
            { label: 'Customers', route: '/customer-management' },
            { label: 'Vendors', route: '/vendors' },
        
          ],
        },
        {
          items: [
            { label: 'Inventory', route: '/inventory' },
              { label: 'Appointments', route: '/appointment-management' },
            { label: 'Reviews', route: '/reviews' },
           
          ],
        },
        {
          items: [
             { label: 'Purchase Invoices', route: '/purchase-invoices' },
            { label: 'Sales Invoices', route: '/sales-invoices' },
            {label:'AI', route:'/gearbox-ai'}
          
           
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
    } else if (isStaff){
        this.menuGroups = [
        {
          items: [
            { label: 'Dashboard', route: '/dashboard' },
        
            { label: 'Customers', route: '/customer-management' },
        
        
          ],
        },
        {
          items: [
            { label: 'Inventory', route: '/inventory' },
              { label: 'Appointments', route: '/appointment-management' },
            { label: 'Reviews', route: '/reviews' },
           
          ],
        },
        {
          items: [
          
            { label: 'Sales Invoices', route: '/sales-invoices' },
            {label:'AI', route:'/gearbox-ai'}
          
           
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
