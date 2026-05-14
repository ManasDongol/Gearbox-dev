import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { NewService, Service } from '../../models/service.model';

@Injectable({
  providedIn: 'root'
})
export class ServiceService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Service`;

  getAll(): Observable<Service[]> {
    return this.http.get<Service[]>(this.apiUrl);
  }

  getById(id: string): Observable<Service> {
    return this.http.get<Service>(`${this.apiUrl}/${id}`);
  }

  add(service: NewService): Observable<Service> {
    return this.http.post<Service>(this.apiUrl, service);
  }

  update(id: string, service: Service): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, service);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
