import { CommonModule } from '@angular/common';
import { Component, HostListener, OnInit, inject } from '@angular/core';
import { NotificationItem } from '../../../core/models/notification.model';
import { Auth } from '../../../core/services/auth/auth';
import { NotificationService } from '../../../core/services/notification/notification.service';
import { SignalRService } from '../../../core/services/SignalR/signalr.service';

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './topbar.html',
  styleUrl: './topbar.css',
})
export class Topbar implements OnInit {
  private notificationService = inject(NotificationService);
  auth = inject(Auth);
  private signalR = inject(SignalRService);

  notifications: NotificationItem[] = [];
  isOpen = false;
  isLoading = false;

 ngOnInit() {
  this.loadNotifications();

  this.signalR.startConnection();

  this.signalR.onNotification((notif: NotificationItem) => {
    this.notifications.unshift(notif);

    // keep only latest 10
    this.notifications = this.notifications.slice(0, 10);
  });
}

  toggleNotifications(event: MouseEvent) {
    event.stopPropagation();
    this.isOpen = !this.isOpen;

    if (this.isOpen) {
      this.loadNotifications();
    }
  }

  @HostListener('document:click')
  closeNotifications() {
    this.isOpen = false;
  }

  onPanelClick(event: MouseEvent) {
    event.stopPropagation();
  }

  get initials(): string {
    const source = this.auth.user?.name || this.auth.user?.email || 'User';
    return source
      .split(/[.@\s_-]+/)
      .filter(Boolean)
      .slice(0, 2)
      .map(part => part[0].toUpperCase())
      .join('');
  }

  formatTime(value: string): string {
    if (!value) return '';

    const date = new Date(value);
    const seconds = Math.floor((Date.now() - date.getTime()) / 1000);

    if (seconds < 60) return 'Just now';
    if (seconds < 3600) return `${Math.floor(seconds / 60)} min ago`;
    if (seconds < 86400) return `${Math.floor(seconds / 3600)} hr ago`;

    return date.toLocaleDateString();
  }

  private loadNotifications() {
    this.isLoading = true;
    this.notificationService.getRecent().subscribe({
      next: notifications => {
        this.notifications = notifications.slice(0, 10);
        this.isLoading = false;
      },
      error: () => {
        this.notifications = [];
        this.isLoading = false;
      },
    });
  }
}
