import { Injectable, signal } from '@angular/core';
import { Notification } from './notification.model';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  notifications = signal<Notification[]>([]);
  private idCounter = 0;

  addSuccessNotification(content: string): void {
    this.notifications().push({
      id: this.idCounter++,
      type: 'success',
      content: content,
    });
  }

  addErrorNotification(content: string): void {
    this.notifications().push({
      id: this.idCounter++,
      type: 'error',
      content: content,
    });
  }

  removeNotification(id: number): void {
    this.notifications.update((notifications) => {
      const updated = notifications.filter(
        (notification) => notification.id !== id,
      );
      if (notifications.length === updated.length) {
        console.warn(`Notification with id ${id} not found`);
      }
      return updated;
    });
  }
}
