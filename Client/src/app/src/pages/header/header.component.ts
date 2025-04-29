import { Component, computed, effect, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../authentification/auth.service';
import { StudentInfoService } from '../studentInfo.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
})
export class HeaderComponent {
  authService = inject(AuthService);
  studentInfoService = inject(StudentInfoService);
  student: string = 'Anonymous';

  constructor() {
    effect(() => {
      const student = this.studentInfoService.student();
      this.student = `${student?.name} ${student?.surname} ${student?.group}`;
    });
  }
}
