import { Routes } from '@angular/router';

import { Login } from './features/auth/login/login';
import { Register } from './features/auth/register/register';

import { PurchaseInvoices } from './features/purchase-invoices/purchase-invoices';
import { StaffManagement } from './features/staff-management/staff-management';
import { CustomerManagement } from './features/customer-management/customer-management';
import { Inventory } from './features/inventory/inventory';
import { AppointmentManagement } from './features/appointment-management/appointment-management';
import { HomePage } from './features/home-page/home-page';
import { Dashboard } from './features/dashboard/dashboard';

import { authGuard } from './core/guards/Auth/auth-guard';
import { adminGuard } from './core/guards/AdminGuard/admin-guard';
import { staffGuard } from './core/guards/StaffGuard/staff-guard';

export const routes: Routes = [

  // Public routes
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'home', component: HomePage },

  // Logged-in users
  {
    path: 'dashboard',
    component: Dashboard,
    canMatch: [staffGuard]
  },

  // Admin only
  {
    path: 'purchase-invoices',
    component: PurchaseInvoices,
    canMatch: [adminGuard]
  },

  {
    path: 'staff-management',
    component: StaffManagement,
    canMatch: [adminGuard]
  },

  // Staff + Admin
  {
    path: 'customer-management',
    component: CustomerManagement,
    canMatch: [staffGuard]
  },

  {
    path: 'inventory',
    component: Inventory,
    canMatch: [staffGuard]
  },

  {
    path: 'appointment-management',
    component: AppointmentManagement,
    canMatch: [staffGuard]
  },

  // Default route
  { path: '', redirectTo: 'home', pathMatch: 'full' },

  // Optional fallback
  { path: '**', redirectTo: 'home' }
];