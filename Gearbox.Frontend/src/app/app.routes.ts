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

import { roleGuard } from './core/guards/RoleGuard/role-guard';

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