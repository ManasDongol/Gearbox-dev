import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { PurchaseInvoice, NewPurchaseInvoice } from '../../models/purchase-invoice.model';

@Injectable({
  providedIn: 'root'
})
export class PurchaseInvoiceService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/PurchaseInvoice`;

  getAll(): Observable<PurchaseInvoice[]> {
    return this.http.get<PurchaseInvoice[]>(this.apiUrl);
  }

  getById(id: string): Observable<PurchaseInvoice> {
    return this.http.get<PurchaseInvoice>(`${this.apiUrl}/${id}`);
  }

  add(invoice: NewPurchaseInvoice): Observable<PurchaseInvoice> {
    return this.http.post<PurchaseInvoice>(this.apiUrl, invoice);
  }

  update(id: string, invoice: PurchaseInvoice): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, invoice);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
