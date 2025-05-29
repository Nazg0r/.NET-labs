import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { debounceTime } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-registration',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css', '../../../../app.component.css'],
})
export class RegistrationComponent implements OnInit {
  private destroyRef = inject(DestroyRef);
  private httpClient = inject(HttpClient);
  private router = inject(Router);

  regForm = new FormGroup({
    username: new FormControl<string>('', {
      validators: [Validators.required, Validators.minLength(6)],
    }),
    name: new FormControl<string>('', {
      validators: [Validators.required],
    }),
    surname: new FormControl<string>('', {
      validators: [Validators.required],
    }),
    group: new FormControl<string>('', {
      validators: [
        Validators.required,
        Validators.minLength(5),
        Validators.maxLength(6),
      ],
    }),
    password: new FormControl<string>('', {
      validators: [Validators.required, Validators.minLength(8)],
    }),
  });

  ngOnInit(): void {
    const savedData = window.localStorage.getItem('userData');

    if (savedData) {
      const formData = JSON.parse(savedData);
      this.regForm.patchValue({
        username: formData.username,
        name: formData.name,
        surname: formData.surname,
        group: formData.group,
      });
    }

    const subscription = this.regForm.valueChanges
      .pipe(debounceTime(500))
      .subscribe({
        next: (value) => {
          window.localStorage.setItem(
            'userData',
            JSON.stringify({
              username: value.username,
              name: value.name,
              surname: value.surname,
              group: value.group,
            }),
          );
        },
      });

    this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }

  isControlInvalid(control: FormControl): boolean {
    return control.touched && control.invalid && control.dirty;
  }

  onSubmit() {
    const userData = {
      username: this.regForm.value.username,
      name: this.regForm.value.name,
      surname: this.regForm.value.surname,
      group: this.regForm.value.group,
      password: this.regForm.value.password,
    };

    this.httpClient
      .post(`${environment.apiUrl}/api/student/register`, userData, {
        observe: 'response',
      })
      .subscribe({
        next: (resp) => {
          if (resp.status === 201) {
            this.router.navigate(['/login']);
          }
        },
      });
  }
}
