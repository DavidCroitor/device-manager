import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ApiService } from '../../../services/api';
import { AuthService } from '../../../services/auth.service';
import { Device } from '../../../models';

@Component({
  selector: 'app-my-devices',
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
          <tr><td colspan="4">You have no devices assigned.</td></tr>
        }
      </tbody>
    </table>
  `,
  styleUrl: '../device-list.css'
})
export class MyDevicesComponent implements OnInit {
  private api = inject(ApiService);
  private auth = inject(AuthService);

  devices: Device[] = [];

  ngOnInit() {
    this.loadDevices();
  }

  loadDevices() {
    const currentUser = this.auth.currentUser();
    this.api.getMyDevices().subscribe(data => {
      this.devices = data.filter(d => d.userId === currentUser?.id);
    });
  }

  deleteDevice(id: number) {
    if (confirm('Are you sure you want to delete this device?')) {
      this.api.deleteDevice(id).subscribe(() => {
        this.devices = this.devices.filter(d => d.id !== id);
      });
    }
  }
}
