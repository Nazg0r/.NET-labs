import { Component, ElementRef, inject, input, viewChild } from '@angular/core';
import { Notification } from './notification.model';
import { NotificationService } from './notification.service';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [],
  templateUrl: './notifications.component.html',
  styleUrl: './notifications.component.css',
})
export class NotificationsComponent {
  private host = inject(ElementRef);
  private notificationService = inject(NotificationService);
  autoCloseDelay = 5000;
  private progressBar =
    viewChild.required<ElementRef<HTMLDivElement>>('progressBar');

  details = input<Notification>({
    id: -1,
    type: 'error',
    content: 'undefined',
  });

  constructor() {
    requestAnimationFrame(() => {
      this.host.nativeElement.classList.add('show');
      setTimeout(
        () => this.progressBar().nativeElement.classList.add('finished'),
        0,
      );
    });

    setTimeout(() => {
      this.notificationService.removeNotification(this.details().id);
    }, this.autoCloseDelay);
  }

  onClose() {
    this.notificationService.removeNotification(this.details().id);
  }
}
