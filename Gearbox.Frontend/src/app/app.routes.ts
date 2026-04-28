import { Routes } from '@angular/router';
import { Login } from './features/auth/login/login';
import { Register } from './features/auth/register/register';
import { PurchaseInvoices } from './features/purchase-invoices/purchase-invoices';
import { StaffManagement } from './features/staff-management/staff-management';
import { CustomerManagement } from './features/customer-management/customer-management';
import { Inventory } from './features/inventory/inventory';

export const routes: Routes = [
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'purchase-invoices', component: PurchaseInvoices },
  { path: 'staff-management', component: StaffManagement },
  { path: 'customer-management', component: CustomerManagement },
  { path: 'inventory', component: Inventory },
  { path: '', redirectTo: 'login', pathMatch: 'full' },
];
