import { Injectable, signal } from '@angular/core';

export interface ConfirmCardOptions {
  title?: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  tone?: 'danger' | 'default';
}

interface ActiveConfirmCard extends Required<ConfirmCardOptions> {
  resolve: (confirmed: boolean) => void;
}

@Injectable({ providedIn: 'root' })
export class ConfirmCardService {
  active = signal<ActiveConfirmCard | null>(null);

  confirm(options: ConfirmCardOptions): Promise<boolean> {
    const current = this.active();
    if (current) {
      current.resolve(false);
    }

    return new Promise((resolve) => {
      this.active.set({
        title: options.title ?? 'Please confirm',
        message: options.message,
        confirmText: options.confirmText ?? 'OK',
        cancelText: options.cancelText ?? 'Cancel',
        tone: options.tone ?? 'danger',
        resolve,
      });
    });
  }

  accept() {
    this.close(true);
  }

  dismiss() {
    this.close(false);
  }

  private close(confirmed: boolean) {
    const current = this.active();
    if (!current) return;

    this.active.set(null);
    current.resolve(confirmed);
  }
}
