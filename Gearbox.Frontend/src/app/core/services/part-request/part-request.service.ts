import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { NewPartRequest, PartRequest } from '../../models/part-request.model';

@Injectable({
  providedIn: 'root',
})
export class PartRequestService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/PartRequest`;

  getAll(): Observable<PartRequest[]> {
    return this.http.get<PartRequest[]>(this.apiUrl);
  }

  getById(id: string): Observable<PartRequest> {
    return this.http.get<PartRequest>(`${this.apiUrl}/${id}`);
  }

  add(request: NewPartRequest): Observable<PartRequest> {
    return this.http.post<PartRequest>(this.apiUrl, request);
  }

  update(id: string, request: PartRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
