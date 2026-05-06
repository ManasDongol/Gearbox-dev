import { Component, inject } from '@angular/core';
import { RouterLink,Router } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';// Adjust path as needed

@Component({
  selector: 'app-register',
  standalone: true,
  // Add ReactiveFormsModule to your imports
  imports: [RouterLink, ReactiveFormsModule], 
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  private fb = inject(FormBuilder);
  private http = inject(HttpClient);
private router = inject(Router);
  // Define the form structure
  registerForm: FormGroup = this.fb.group({
    userName: ['', [Validators.required, Validators.minLength(3)]],
    address: ['', [Validators.required]],
    firstName: ['', [Validators.required]],
    lastName: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    phoneNumber: ['', [Validators.required]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  onSubmit() {
    if (this.registerForm.valid) {
      const payload = this.registerForm.value;
      const url = `${environment.apiUrl}/customer`;

      console.log('Submitting to:', url, payload);

      this.http.post(url, payload).subscribe({
        next: (response) => {
          console.log('Registration successful', response);
          this.router.navigate(['/login']);
          
        },
        error: (error) => {
          console.error('Registration failed', error);
        }
      });
    } else {
      // Mark fields as touched to trigger validation messages in UI
      this.registerForm.markAllAsTouched();
    }
  }
}