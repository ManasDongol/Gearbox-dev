import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Part, NewPart } from '../../Models/part.model';

@Injectable({
  providedIn: 'root'
})
export class PartService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Part`;

  getAll(): Observable<Part[]> {
    return this.http.get<Part[]>(this.apiUrl);
  }

  getById(id: string): Observable<Part> {
    return this.http.get<Part>(`${this.apiUrl}/${id}`);
  }

  add(part: NewPart): Observable<Part> {
    return this.http.post<Part>(this.apiUrl, part);
  }

  update(id: string, part: Part): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, part);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
