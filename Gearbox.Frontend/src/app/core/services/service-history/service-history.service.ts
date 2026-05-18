import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';

export interface ServiceHistory {
  id: string;
  customerId: string;
  vehicleId: string;
  serviceId: string;
  serviceDate: string;
  notes: string;
  totalCost: number;
}

@Injectable({
  providedIn: 'root'
})
export class ServiceHistoryService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/ServiceHistory`;

  getAll(): Observable<ServiceHistory[]> {
    return this.http.get<ServiceHistory[]>(this.apiUrl);
  }

  getById(id: string): Observable<ServiceHistory> {
    return this.http.get<ServiceHistory>(`${this.apiUrl}/${id}`);
  }
}
