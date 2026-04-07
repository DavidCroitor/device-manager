import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { User } from '../models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private baseUrl = 'http://localhost:5000/api/users';

  currentUser = signal<User | null>(null);

  constructor() {
    const userJson = localStorage.getItem('currentUser');
    if (userJson) {
      this.currentUser.set(JSON.parse(userJson));
    }
  }

  register(data: any): Observable<User> {
    return this.http.post<User>(`${this.baseUrl}/register`, data);
  }

  login(credentials: any): Observable<User> {
    return this.http.post<User>(`${this.baseUrl}/login`, credentials).pipe(
      tap((user) => this.setAuth(user))
    );
  }

  logout() {
    localStorage.removeItem('currentUser');
    localStorage.removeItem('token');
    this.currentUser.set(null);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  private setAuth(response: any) {
    if (!response) return;

    // Support both direct user object and { currentUser, token } wrapper
    let userToSave = response;
    let token = '';

    if (response.currentUser) {
      userToSave = response.currentUser;
    }

    if (response.token) {
      token = response.token;
    } else if (response.currentUser && response.currentUser.token) {
      token = response.currentUser.token;
    }

    // Keep token in the user object just in case components access it
    userToSave.token = token;

    localStorage.setItem('currentUser', JSON.stringify(userToSave));
    localStorage.setItem('token', token);
    this.currentUser.set(userToSave);
  }
}
