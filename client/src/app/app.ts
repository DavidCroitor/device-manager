import { Component, inject } from '@angular/core';
import { RouterOutlet, Router } from '@angular/router';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  styleUrls: ['./app.css'],
  template: `
    <div class="app-container">
      <div class="header">
        <h1>Device Management System</h1>
        @if (auth.currentUser()) {
          <button (click)="logout()" class="logout-btn">Logout</button>
        }
      </div>
      <hr>
      <router-outlet></router-outlet>
    </div>
  `
})
export class AppComponent {
  auth = inject(AuthService);
  private router = inject(Router);

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}

