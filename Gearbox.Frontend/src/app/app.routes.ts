import { Routes } from '@angular/router';
import { Login } from './features/auth/login/login';
import { Register } from './features/auth/register/register';
import { PurchaseInvoices } from './features/purchase-invoices/purchase-invoices';

export const routes: Routes = [
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'purchase-invoices', component: PurchaseInvoices },
  { path: '', redirectTo: 'login', pathMatch: 'full' },
];
