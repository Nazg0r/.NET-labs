import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Login } from './login.model';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private isAuthenticated: boolean = false;
  private router = inject(Router);
  constructor(private httpClient: HttpClient) {}

  get getAuthenticationStatus() {
    return this.isAuthenticated;
  }

  login(credentials: Login) {
    return this.httpClient
      .post<{ token: string; expiresDate: string }>(
        'http://localhost:5000/api/student/login',
        credentials,
        {
          observe: 'response',
        },
      )
      .subscribe({
        next: (resp) => {
          if (resp.status === 200) {
            this.isAuthenticated = true;

            window.localStorage.setItem(
              'token',
              JSON.stringify({
                token: resp.body?.token,
                expiresDate: resp.body?.expiresDate,
              }),
            );

            console.log(this.isAuthenticated);
            this.router.navigateByUrl('/workspace');
          }
        },
        error: (err) => {
          console.log(`Login failed. ${err}`);
        },
      });
  }

  logout() {
    this.isAuthenticated = false;
    window.localStorage.removeItem('token');
  }
}
