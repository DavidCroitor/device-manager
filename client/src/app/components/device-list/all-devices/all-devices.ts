import { Component, Input, Output, EventEmitter, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ApiService } from '../../../services/api';
import { Device } from '../../../models';

@Component({
  selector: 'app-all-devices',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <table>
      <thead>
        <tr>
          <th>Name</th>
          <th>Type</th>
          <th>Assigned User</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        @for (device of devices; track device.id) {
          <tr>
            <td>{{ device.name }}</td>
            <td>{{ device.type }}</td>
            <td>
              @if (device.userId) {
                {{ device.userName }} ({{ device.userRole }}) - {{ device.userLocation }}
              } @else {
                <span class="unassigned-text">Unassigned</span>
              }
            </td>
            <td>
              <button [routerLink]="['/devices', device.id]" class="btn btn-details">Details</button>
              <button [routerLink]="['/devices/edit', device.id]" class="btn btn-edit">Edit</button>
              <button (click)="deleteDevice(device.id)" class="btn btn-delete">Delete</button>
            </td>
          </tr>
        } @empty {
          <tr><td colspan="4">No devices found in this list.</td></tr>
        }
      </tbody>
    </table>
  `,
  styleUrl: '../device-list.css'
})
export class AllDevicesComponent implements OnInit {
  private api = inject(ApiService);
  devices: Device[] = [];

  ngOnInit() {
    this.loadDevices();
  }

  loadDevices() {
    this.api.getDevices().subscribe(data => this.devices = data);
  }

  deleteDevice(id: number) {
    if (confirm('Are you sure you want to delete this device?')) {
      this.api.deleteDevice(id).subscribe(() => {
        this.devices = this.devices.filter(d => d.id !== id);
      });
    }
  }
}
