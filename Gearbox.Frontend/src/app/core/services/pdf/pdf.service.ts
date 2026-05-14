import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PdfService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Pdf`;

  generateFinancialReport(): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/financial-report`, { responseType: 'blob' });
  }

  generateCustomerReport(): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/customer-report`, { responseType: 'blob' });
  }

  generateRegularCustomersReport(): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/regular-customers-report`, { responseType: 'blob' });
  }

  generateHighSpendersReport(): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/high-spenders-report`, { responseType: 'blob' });
  }

  generatePendingCreditsReport(): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/pending-credits-report`, { responseType: 'blob' });
  }
}
