import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ApiService } from '../../services/api';
import { Device } from '../../models';

@Component({
  selector: 'app-device-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <h2>Devices</h2>
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
            <td>{{ device.userName }} ({{ device.userRole }}) - {{ device.userLocation }}</td>
            <td>
              <button [routerLink]="['/devices', device.id]" class="btn btn-details">Details</button>
              <button [routerLink]="['/devices/edit', device.id]" class="btn btn-edit">Edit</button>
              <button (click)="deleteDevice(device.id)" class="btn btn-delete">Delete</button>
            </td>
          </tr>
        } @empty {
          <tr><td colspan="4">No devices found.</td></tr>
        }
      </tbody>
    </table>
    <div class ="actions-container">
      <button routerLink="/devices/new" class="btn btn-create">Create New Device</button>
    </div>
    `,
  styleUrl: './device-list.css'
})
export class DeviceListComponent implements OnInit {
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
        // Refresh the list after deletion
        this.devices = this.devices.filter(d => d.id !== id);
      });
    }
  }
}
