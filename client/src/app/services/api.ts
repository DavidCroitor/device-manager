import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Device, User } from '../models';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private http = inject(HttpClient);
  private baseUrl = 'http://localhost:5000/api';

  // --- Devices ---
  getDevices(pageNumber: number = 1, pageSize: number = 10): Observable<Device[]> {
    return this.http.get<Device[]>(`${this.baseUrl}/devices?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }

  searchDevices(queryString: string, pageNumber: number = 1, pageSize: number = 10): Observable<Device[]> {
    return this.http.get<Device[]>(`${this.baseUrl}/devices/search?queryString=${queryString}&pageNumber=${pageNumber}&pageSize=${pageSize}`);
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

  getMyDevices(pageNumber: number = 1, pageSize: number = 10): Observable<Device[]> {
    return this.http.get<Device[]>(`${this.baseUrl}/devices/my-devices?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }

  getUnassignedDevices(pageNumber: number = 1, pageSize: number = 10): Observable<Device[]> {
    return this.http.get<Device[]>(`${this.baseUrl}/devices/unassigned?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }

  generateDescription(id: number): Observable<{ description: string }> {
    return this.http.get<{ description: string }>(`${this.baseUrl}/devices/description/${id}`);
  }

  // --- Users ---
  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.baseUrl}/users`);
  }
}
