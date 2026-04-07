import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  styleUrls: ['./login.css'],
  template: `
    <div class="auth-container">
      <h2>Login</h2>
      <form [formGroup]="loginForm" (ngSubmit)="onSubmit()">
        <div *ngIf="errorMessage" class="error-message" style="color: red; margin-bottom: 10px;">
          {{ errorMessage }}
        </div>
        <label>Email</label>
        <input type="email" formControlName="email" required />

        <label>Password</label>
        <input type="password" formControlName="password" required />

        <button type="submit" [disabled]="loginForm.invalid || loading">Login</button>
      </form>
      <p>Don't have an account? <a routerLink="/register">Register</a></p>
    </div>
  `
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  loginForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required]
  });

  loading = false;
  errorMessage = '';

  onSubmit() {
    this.errorMessage = '';
    if (this.loginForm.valid) {
      this.loading = true;
      this.auth.login(this.loginForm.value).subscribe({
        next: () => {
          this.router.navigate(['/']);
        },
        error: (err) => {
          console.error('Login failed', err);
          if (err.status === 401) {
            this.errorMessage = 'Invalid email or password. Please try again.';
          } else {
            this.errorMessage = 'An error occurred during login. Please try again later.';
          }
          this.loading = false;
        }
      });
    }
  }
}
