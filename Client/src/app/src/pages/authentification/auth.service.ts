import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Login } from './login.model';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private isAuthenticated = signal<boolean>(false);
  private router = inject(Router);
  constructor(private httpClient: HttpClient) {
    this.checkLocalStorageToken();
  }

  get getAuthenticationStatus() {
    return this.isAuthenticated;
  }

  checkLocalStorageToken() {
    const tokenInfo = localStorage.getItem('token');
    try {
      if (tokenInfo) {
        const { token, expiresDate } = JSON.parse(tokenInfo);
        const isTokenValid = new Date(expiresDate) > new Date();

        if (isTokenValid) this.isAuthenticated.set(true);
        else this.logout();
      }
    } catch (err) {
      this.logout();
      console.error(err);
    }
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
            this.isAuthenticated.set(true);

            window.localStorage.setItem(
              'token',
              JSON.stringify({
                token: resp.body?.token,
                expiresDate: resp.body?.expiresDate,
              }),
            );
            this.router.navigateByUrl('/workspace');
          }
        },
        error: (err) => {
          console.log(`Login failed. ${err}`);
        },
      });
  }

  logout() {
    this.isAuthenticated.set(false);
    window.localStorage.removeItem('token');
  }
}
