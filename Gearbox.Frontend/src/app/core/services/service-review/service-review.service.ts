import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { NewServiceReview, ServiceReview } from '../../models/service-review.model';

@Injectable({
  providedIn: 'root'
})
export class ServiceReviewService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/ServiceReview`;

  getAll(): Observable<ServiceReview[]> {
    return this.http.get<ServiceReview[]>(this.apiUrl);
  }

  getById(id: string): Observable<ServiceReview> {
    return this.http.get<ServiceReview>(`${this.apiUrl}/${id}`);
  }

  getByCustomerId(customerId: string): Observable<ServiceReview[]> {
    return this.http.get<ServiceReview[]>(`${this.apiUrl}/customer/${customerId}`);
  }

  getByAppointmentId(appointmentId: string): Observable<ServiceReview[]> {
    return this.http.get<ServiceReview[]>(`${this.apiUrl}/appointment/${appointmentId}`);
  }

  getByServiceId(serviceId: string): Observable<ServiceReview[]> {
    return this.http.get<ServiceReview[]>(`${this.apiUrl}/service/${serviceId}`);
  }

  add(review: NewServiceReview): Observable<ServiceReview> {
    return this.http.post<ServiceReview>(this.apiUrl, review);
  }

  update(id: string, review: ServiceReview): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, review);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
