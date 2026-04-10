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
    <div class="auth-wrapper">
      <div class="auth-card">
        <div class="auth-header">
          <h2>Welcome Back</h2>
          <p>Sign in to your account to continue</p>
        </div>

        <form [formGroup]="loginForm" (ngSubmit)="onSubmit()">
          <div *ngIf="errorMessage" class="error-banner">
            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
              <path d="M8.982 1.566a1.13 1.13 0 0 0-1.96 0L.165 13.233c-.457.778.091 1.767.98 1.767h13.713c.889 0 1.438-.99.98-1.767L8.982 1.566zM8 5c.535 0 .954.462.9.995l-.35 3.507a.552.552 0 0 1-1.1 0L7.1 5.995A.905.905 0 0 1 8 5zm.002 6a1 1 0 1 1 0 2 1 1 0 0 1 0-2z"/>
            </svg>
            <span>{{ errorMessage }}</span>
          </div>

          <div class="form-group">
            <label>Email Address</label>
            <input type="email" formControlName="email" placeholder="name@company.com" required [class.error-input]="f['email'].invalid && f['email'].touched" />
            <div *ngIf="f['email'].invalid && f['email'].touched" class="error-text">
              <small *ngIf="f['email'].errors?.['required']">Email is required.</small>
              <small *ngIf="f['email'].errors?.['email']">Please enter a valid email address.</small>
            </div>
          </div>

          <div class="form-group">
            <label>Password</label>
            <input type="password" formControlName="password" placeholder="••••••••" required [class.error-input]="f['password'].invalid && f['password'].touched" />
            <div *ngIf="f['password'].invalid && f['password'].touched" class="error-text">
              <small *ngIf="f['password'].errors?.['required']">Password is required.</small>
            </div>
          </div>

          <button type="submit" class="btn-primary" [disabled]="loginForm.invalid || loading">
            {{ loading ? 'Signing in...' : 'Sign In' }}
          </button>
        </form>

        <div class="auth-footer">
          <p>Don't have an account? <a routerLink="/register">Create one now</a></p>
        </div>
      </div>
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

  get f() {
    return this.loginForm.controls;
  }

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
