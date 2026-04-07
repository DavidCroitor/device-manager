import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Device, User } from '../models';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private http = inject(HttpClient);
  private baseUrl = 'http://localhost:5000/api';

  // --- Devices ---
  getDevices(): Observable<Device[]> {
    return this.http.get<Device[]>(`${this.baseUrl}/devices`);
  }

  getDevice(id: number): Observable<Device> {
    return this.http.get<Device>(`${this.baseUrl}/devices/${id}`);
  }

  createDevice(device: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/devices`, device);
  }

  updateDevice(id: number, device: any): Observable<any> {
    return this.http.patch(`${this.baseUrl}/devices/${id}`, device);
  }

  deleteDevice(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/devices/${id}`);
  }

  getMyDevices(): Observable<Device[]> {
    return this.http.get<Device[]>(`${this.baseUrl}/devices/my-devices`);
  }

  getUnassignedDevices(): Observable<Device[]> {
    return this.http.get<Device[]>(`${this.baseUrl}/devices/unassigned`);
  }

  // --- Users ---
  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.baseUrl}/users`);
  }
}
