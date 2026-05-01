import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { LoginService } from '../../../core/services/login/login';
import { Auth } from '../../../core/services/auth/auth';

@Component({
  selector: 'app-login',
  imports: [RouterLink, ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private auth = inject(Auth);

  authService = inject(LoginService);

  errorMessage = '';

  form = this.fb.nonNullable.group({
    username: ['', [Validators.required]],
    password: ['', Validators.required],
  });

onSubmit() {
  if (this.form.invalid) {
    this.form.markAllAsTouched();
    return;
  }

  this.authService.login(this.form.getRawValue()).subscribe({
    next: () => {
      console.log('Logged in');

      this.auth.loadUser();
      this.router.navigate(['/dashboard']);
    },
    error: () => {
      this.errorMessage = 'Invalid credentials';
    }
  });
}
}