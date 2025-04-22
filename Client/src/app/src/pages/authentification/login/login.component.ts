import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';
import { Login } from '../login.model';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css', '../../../../app.component.css'],
})
export class LoginComponent implements OnInit {
  private httpClient = inject(HttpClient);
  private router = inject(Router);
  private authService = inject(AuthService);
  private destroyRef = inject(DestroyRef);

  loginForm = new FormGroup({
    username: new FormControl('', {
      validators: [Validators.required, Validators.minLength(6)],
    }),
    password: new FormControl('', {
      validators: [Validators.required, Validators.minLength(8)],
    }),
  });

  isControlInvalid(control: FormControl): boolean {
    return control.touched && control.invalid && control.dirty;
  }

  ngOnInit() {
    const storedData = window.localStorage.getItem('userData');

    if (storedData) {
      const formData = JSON.parse(storedData);
      this.loginForm.patchValue({
        username: formData.username,
      });
    }
  }

  onSubmit() {
    const credentials: Login = {
      username: this.loginForm.value.username,
      password: this.loginForm.value.password,
    };
    const subscription = this.authService.login(credentials);

    this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }
}
