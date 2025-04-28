import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { WelcomeComponent } from './src/pages/welcome/welcome.component';
import { HeaderComponent } from './src/pages/header/header.component';
import { RegistrationComponent } from './src/pages/authentification/registration/registration.component';
import { BackgroundComponent } from './src/pages/background/background.component';
import { LoginComponent } from './src/pages/authentification/login/login.component';
import { NotificationService } from './src/pages/notifications/notification.service';
import { NotificationsComponent } from './src/pages/notifications/notifications.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    WelcomeComponent,
    HeaderComponent,
    RegistrationComponent,
    BackgroundComponent,
    LoginComponent,
    NotificationsComponent,
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  notificationService = inject(NotificationService);
}
