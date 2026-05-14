import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { SalesInvoice, NewSalesInvoice } from '../../models/sales-invoice.model';

@Injectable({
  providedIn: 'root'
})
export class SalesInvoiceService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/SalesServicesInvoice`;

  getAll(): Observable<SalesInvoice[]> {
    return this.http.get<SalesInvoice[]>(this.apiUrl);
  }

  getById(id: string): Observable<SalesInvoice> {
    return this.http.get<SalesInvoice>(`${this.apiUrl}/${id}`);
  }

  add(invoice: NewSalesInvoice): Observable<SalesInvoice> {
    return this.http.post<SalesInvoice>(this.apiUrl, invoice);
  }

  update(id: string, invoice: SalesInvoice): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, invoice);
  }

  markAsPaid(id: string): Observable<SalesInvoice> {
    return this.http.patch<SalesInvoice>(`${this.apiUrl}/${id}/mark-paid`, {});
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
