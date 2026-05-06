import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { LoginService } from '../../../core/services/login/login';
import { Auth } from '../../../core/services/auth/auth';
import { switchMap } from 'rxjs';

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

  this.authService.login(this.form.getRawValue()).pipe(
    switchMap(() => this.auth.loadUser())
  ).subscribe({
    next: (user) => {
      if (!user) {
        this.errorMessage = 'Could not load user info';
        return;
      }
      if (this.auth.hasAnyRole(['Admin', 'Staff'])) {
        this.router.navigate(['/dashboard']);
      } else {
        console.log(this.auth.getRole());
        this.router.navigate([`/user-dashboard`]);
      }
    },
    error: () => {
      this.errorMessage = 'Invalid credentials';
    }
  });
}
}