import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { LoginService } from '../../../core/services/login/login';
import { Auth } from '../../../core/services/auth/auth';
import { switchMap } from 'rxjs';
import { ToastService } from '../../../shared/components/toast/toast.service';
import { Spinner } from '../../../shared/components/spinner/spinner';
@Component({
  selector: 'app-login',
  standalone:true,
  imports: [RouterLink, ReactiveFormsModule,Spinner],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  isLoading: boolean =false;
  showPassword = false;
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private auth = inject(Auth);
  private toast = inject(ToastService);

  authService = inject(LoginService);

  errorMessage = '';

  form = this.fb.nonNullable.group({
    username: ['', [Validators.required]],
    password: ['', Validators.required],
  });

  hasError(controlName: 'username' | 'password', error: string): boolean {
    const control = this.form.controls[controlName];
    return control.touched && control.hasError(error);
  }

onSubmit() {
  if (this.form.invalid) {
    this.form.markAllAsTouched();
    return;
  }

  this.isLoading =true;

  this.authService.login(this.form.getRawValue()).pipe(
    switchMap(() => this.auth.loadUser())
  ).subscribe({
    next: (user) => {
      if (!user) {
        this.errorMessage = 'Could not load user info';
         this.isLoading =false;
        return;
      }
      if (this.auth.hasAnyRole(['Admin', 'Staff'])) {
         this.isLoading =false;
        this.toast.success("login successful!, re-directing","");
        this.router.navigate(['/dashboard']);
      } else {
        console.log(this.auth.getRole());
         this.isLoading =false;
         this.toast.success("login successful!, re-directing","");
        this.router.navigate([`/user-dashboard`]);
      }
    },
    error: () => {
       this.isLoading =false;
       this.toast.error("login unsuccessful please try again!","");
      this.errorMessage = 'Invalid credentials';
    }
  });
}
}
