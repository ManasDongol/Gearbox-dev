import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Staff, NewStaff } from '../../models/staff.model';

@Injectable({
  providedIn: 'root'
})
export class StaffService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Staff`;

  getAll(): Observable<Staff[]> {
    return this.http.get<Staff[]>(this.apiUrl);
  }

  getById(id: string): Observable<Staff> {
    return this.http.get<Staff>(`${this.apiUrl}/${id}`);
  }

  add(staff: NewStaff): Observable<void> {
    return this.http.post<void>(this.apiUrl, staff);
  }

  update(id: string, staff: Staff): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, staff);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  promoteToAdmin(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/promote-admin`, {});
  }
}
