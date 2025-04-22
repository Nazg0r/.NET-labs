import { Routes } from '@angular/router';
import { RegistrationComponent } from './src/pages/authentification/registration/registration.component';
import { WelcomeComponent } from './src/pages/welcome/welcome.component';
import { LoginComponent } from './src/pages/authentification/login/login.component';
import { WorkspaceComponent } from './src/pages/workspace/workspace.component';
import { AuthGuardService } from './src/pages/authentification/AuthGuard.service';

export const routes: Routes = [
  {
    path: '',
    component: WelcomeComponent,
  },
  {
    path: 'register',
    component: RegistrationComponent,
  },
  {
    path: 'login',
    component: LoginComponent,
  },
  {
    path: 'workspace',
    component: WorkspaceComponent,
    canActivate: [AuthGuardService],
  },
];
