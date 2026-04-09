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
    <div class="auth-wrapper">
      <div class="auth-card register-card">
        <div class="auth-header">
          <h2>Create Account</h2>
          <p>Sign up to start managing your devices</p>
        </div>

        <form [formGroup]="registerForm" (ngSubmit)="onSubmit()">
          <div *ngIf="errorMessage" class="error-banner">
            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
              <path d="M8.982 1.566a1.13 1.13 0 0 0-1.96 0L.165 13.233c-.457.778.091 1.767.98 1.767h13.713c.889 0 1.438-.99.98-1.767L8.982 1.566zM8 5c.535 0 .954.462.9.995l-.35 3.507a.552.552 0 0 1-1.1 0L7.1 5.995A.905.905 0 0 1 8 5zm.002 6a1 1 0 1 1 0 2 1 1 0 0 1 0-2z"/>
            </svg>
            <span>{{ errorMessage }}</span>
          </div>

          <div class="form-grid">
            <div class="form-group full-width">
              <label>Full Name</label>
              <input type="text" formControlName="name" placeholder="John Doe" required [class.error-input]="f['name'].invalid && f['name'].touched" />
              <div *ngIf="f['name'].invalid && f['name'].touched" class="error-text">
                <small *ngIf="f['name'].errors?.['required']">Full Name is required.</small>
              </div>
            </div>

            <div class="form-group full-width">
              <label>Email Address</label>
              <input type="email" formControlName="email" placeholder="john@company.com" required [class.error-input]="f['email'].invalid && f['email'].touched" />
              <div *ngIf="f['email'].invalid && f['email'].touched" class="error-text">
                <small *ngIf="f['email'].errors?.['required']">Email is required.</small>
                <small *ngIf="f['email'].errors?.['email']">Please enter a valid email address.</small>
              </div>
            </div>

            <div class="form-group">
              <label>Role</label>
              <input type="text" formControlName="role" placeholder="e.g. Developer" required [class.error-input]="f['role'].invalid && f['role'].touched" />
              <div *ngIf="f['role'].invalid && f['role'].touched" class="error-text">
                <small *ngIf="f['role'].errors?.['required']">Role is required.</small>
              </div>
            </div>

            <div class="form-group">
              <label>Location</label>
              <input type="text" formControlName="location" placeholder="e.g. Cluj-Napoca" required [class.error-input]="f['location'].invalid && f['location'].touched" />
              <div *ngIf="f['location'].invalid && f['location'].touched" class="error-text">
                <small *ngIf="f['location'].errors?.['required']">Location is required.</small>
              </div>
            </div>

            <div class="form-group">
              <label>Password</label>
              <input type="password" formControlName="password" placeholder="••••••••" required [class.error-input]="f['password'].invalid && f['password'].touched" />
              <div *ngIf="f['password'].invalid && f['password'].touched" class="error-text">
                <small *ngIf="f['password'].errors?.['required']">Password is required.</small>
                <small *ngIf="f['password'].errors?.['minlength']">Password must be at least 6 characters long.</small>
                <small *ngIf="f['password'].errors?.['pattern'] && !f['password'].errors?.['minlength']">Password must contain at least a number, uppercase and lowercase letter.</small>
              </div>
            </div>

            <div class="form-group">
              <label>Confirm Password</label>
              <input type="password" formControlName="confirmPassword" placeholder="••••••••" required [class.error-input]="f['confirmPassword'].invalid && f['confirmPassword'].touched" />
              <div *ngIf="(f['confirmPassword'].invalid || registerForm.errors?.['passwordsMismatch']) && f['confirmPassword'].touched" class="error-text">
                <small *ngIf="f['confirmPassword'].errors?.['required']">Please confirm your password.</small>
                <small *ngIf="registerForm.errors?.['passwordsMismatch'] && !f['confirmPassword'].errors?.['required']">Passwords do not match.</small>
              </div>
            </div>
          </div>

          <button type="submit" class="btn-primary" [disabled]="registerForm.invalid || loading">
            {{ loading ? 'Creating Account...' : 'Create Account' }}
          </button>
        </form>

        <div class="auth-footer">
          <p>Already have an account? <a routerLink="/login">Sign In</a></p>
        </div>
      </div>
    </div>
  `
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  get f() {
    return this.registerForm.controls;
  }

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
    password: ['', [
      Validators.required,
      Validators.minLength(6),
      Validators.pattern(/^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).*$/)
    ]],
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
