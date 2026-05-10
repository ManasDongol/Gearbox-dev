import { Routes } from '@angular/router';

import { Login } from './features/auth/login/login';
import { Register } from './features/auth/register/register';
import { HomePage } from './features/home-page/home-page';
import { Dashboard } from './features/dashboard/dashboard';

import { PurchaseInvoices } from './features/purchase-invoices/purchase-invoices';
import { StaffManagement } from './features/staff-management/staff-management';
import { CustomerManagement } from './features/customer-management/customer-management';
import { Inventory } from './features/inventory/inventory';
import { AppointmentManagement } from './features/appointment-management/appointment-management';
import { VendorManagement } from './features/vendor-management/vendor-management';

import { roleGuard } from './core/guards/RoleGuard/role-guard';
import { ConfirmEmailComponent } from './features/confirm-email/confirm-email';

export const routes: Routes = [

  // Public routes
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login').then(m => m.Login)
  },
  {
    path: 'register',
    loadComponent: () => import('./features/auth/register/register').then(m => m.Register)
  },
  {
    path: 'home',
    loadComponent: () => import('./features/home-page/home-page').then(m => m.HomePage)
  },

  // Logged-in users (Admin + Staff)
  {
    path: 'dashboard',
    canActivate: [roleGuard('Admin', 'Staff')],
    loadComponent: () => import('./features/dashboard/dashboard').then(m => m.Dashboard)
  },

  // Admin only
  {
    path: 'purchase-invoices',
    canActivate: [roleGuard('Admin')],
    loadComponent: () => import('./features/purchase-invoices/purchase-invoices').then(m => m.PurchaseInvoices)
  },

  {
    path: 'sales-invoices',
    canActivate: [roleGuard('Admin', 'Staff')],
    loadComponent: () => import('./features/sales-invoices/sales-invoices').then(m => m.SalesInvoices)
  },

  {
    path: 'staff-management',
    canActivate: [roleGuard('Admin')],
    loadComponent: () => import('./features/staff-management/staff-management').then(m => m.StaffManagement)
  },

  // Staff + Admin
  {
    path: 'customer-management',
    canActivate: [roleGuard('Admin', 'Staff')],
    loadComponent: () => import('./features/customer-management/customer-management').then(m => m.CustomerManagement)
  },

  {
    path: 'inventory',
    canActivate: [roleGuard('Admin', 'Staff')],
    loadComponent: () => import('./features/inventory/inventory').then(m => m.Inventory)
  },

  {
    path: 'appointment-management',
    canActivate: [roleGuard('Admin', 'Staff')],
    loadComponent: () => import('./features/appointment-management/appointment-management').then(m => m.AppointmentManagement)
  },

  {
    path: 'vendors',
    canActivate: [roleGuard('Admin')],
    loadComponent: () => import('./features/vendor-management/vendor-management').then(m => m.VendorManagement)
  },


   {
    path: 'user-dashboard',
    canActivate: [roleGuard('Customer')],
    loadComponent: () => import('./features/user-dashboard/user-dashboard').then(m => m.UserDashboard)
  },
  {
    path: 'my-vehicles',
    canActivate: [roleGuard('Customer')],
    loadComponent: () => import('./features/vehicle/vehicle').then(m => m.Vehicle)
  },
  {
    path: 'vehicles',
    canActivate: [roleGuard('Customer')],
    loadComponent: () => import('./features/vehicle/vehicle').then(m => m.Vehicle)
  },
  {
    path: 'my-appointments',
    canActivate: [roleGuard('Customer')],
    loadComponent: () => import('./features/my-appointments/my-appointments').then(m => m.MyAppointments)
  },
  {
    path: 'my-invoices',
    canActivate: [roleGuard('Customer')],
    loadComponent: () => import('./features/my-invoices/my-invoices').then(m => m.MyInvoices)
  },
  {
    path: 'requests',
    canActivate: [roleGuard('Customer')],
    loadComponent: () => import('./features/part-requests/part-requests').then(m => m.PartRequests)
  },
  {
    path: 'profile',
    canActivate: [roleGuard('Admin', 'Staff', 'Customer')],
    loadComponent: () => import('./features/profile/profile').then(m => m.Profile)
  },
  {
    path: 'gearbox-ai',
    canActivate: [roleGuard('Customer','Staff','Admin')],
    loadComponent: () => import('./features/ai-page/ai-page').then(m => m.GearboxAiPage)
  },

  { path: 'confirm-email', component: ConfirmEmailComponent },
  // Default
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full'
  },

  // Fallback
  {
    path: '**',
    redirectTo: 'home'
  }
];
