import { Component, input } from '@angular/core';

@Component({
  selector: 'app-student-work',
  standalone: true,
  templateUrl: './student-work.component.html',
  styleUrl: './student-work.component.css',
})
export class StudentWorkComponent {
  workName = input('undefined');
}
