import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { NotificationItem } from '../../models/notification.model';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/Notification`;

  getRecent(): Observable<NotificationItem[]> {
    return this.http.get<NotificationItem[]>(`${this.baseUrl}/recent`);
  }
}
