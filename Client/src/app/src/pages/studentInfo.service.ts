import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { jwtDecode } from 'jwt-decode';
import { Student } from './student.model';
import { of } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class StudentInfoService {
  student = signal<Student | null>(null);

  constructor(private httpClient: HttpClient) {}

  loadStudentInfo(): void {
    const tokenInfo = localStorage.getItem('token');
    if (!tokenInfo) return;

    try {
      const token = JSON.parse(tokenInfo).token;
      const payload = jwtDecode<{ UserName: string }>(token);
      if (!payload?.UserName) return;

      this.httpClient
        .get<Student>(`${environment.apiUrl}/api/student/${payload.UserName}`)
        .pipe(
          catchError((error) => {
            console.error('Error loading student:', error);
            return of(null);
          }),
        )
        .subscribe({
          next: (resp) => {
            this.student.set(resp);
          },
        });
    } catch (err) {
      console.error('Failed to parse token:', err);
    }
  }
}
