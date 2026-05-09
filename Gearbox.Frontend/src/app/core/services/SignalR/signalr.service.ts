import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class SignalRService {
  private hubConnection!: signalR.HubConnection;

  startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.apiUrl}/notificationHub`)
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start()
      .then(() => console.log('SignalR Connected'))
      .catch(err => console.error('SignalR error:', err));
  }

  onNotification(callback: (data: any) => void) {
    this.hubConnection.on('ReceiveNotification', callback);
  }
}