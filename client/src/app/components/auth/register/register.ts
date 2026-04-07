import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  styleUrls: ['./register.css'],
  template: `
    <div class="auth-container">
      <h2>Register</h2>
      <form [formGroup]="registerForm" (ngSubmit)="onSubmit()">
        <div *ngIf="errorMessage" class="error-message" style="color: red; margin-bottom: 10px;">
          {{ errorMessage }}
        </div>
        <label>Name</label>
        <input type="text" formControlName="name" required />

        <label>Email</label>
        <input type="email" formControlName="email" required />

        <label>Role</label>
        <input type="text" formControlName="role" required />

        <label>Location</label>
        <input type="text" formControlName="location" required />

        <label>Password</label>
        <input type="password" formControlName="password" required />

        <label>Confirm Password</label>
        <input type="password" formControlName="confirmPassword" required />

        <button type="submit" [disabled]="registerForm.invalid || loading">Register</button>
      </form>
      <p>Already have an account? <a routerLink="/login">Login</a></p>
    </div>
  `
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  passwordsMatch = (group: any) => {
    const password = group.get('password')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;
    return password === confirmPassword ? null : { passwordsMismatch: true };
  }

  registerForm = this.fb.group({
    name: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    role: ['', Validators.required],
    location: ['', Validators.required],
    password: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', Validators.required]
  }, { validators: this.passwordsMatch });

  loading = false;
  errorMessage = '';

  onSubmit() {
    this.errorMessage = '';
    if (this.registerForm.valid) {
      this.loading = true;
      this.auth.register(this.registerForm.value).subscribe({
        next: () => {
          this.router.navigate(['/login']);
        },
        error: (err) => {
          console.error('Registration failed', err);
          if (err.error?.errors) {
            const errorMessages = Object.values(err.error.errors).map((e: any) => e.join(' '));
            this.errorMessage = errorMessages.join(' ');
          } else {
            this.errorMessage = err.error?.message || err.error?.title || 'Registration failed. Please make sure all details are valid.';
          }
          this.loading = false;
        }
      });
    }
  }
}
