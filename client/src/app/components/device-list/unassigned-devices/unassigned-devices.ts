import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ApiService } from '../../../services/api';
import { AuthService } from '../../../services/auth.service';
import { Device } from '../../../models';

@Component({
  selector: 'app-unassigned-devices',
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
              <span class="unassigned-text">Unassigned</span>
            </td>
            <td>
              <button [routerLink]="['/devices', device.id]" class="btn btn-details">Details</button>
              <button (click)="assign(device.id)" class="btn btn-assign">Assign</button>
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
export class UnassignedDevicesComponent implements OnInit {
  private api = inject(ApiService);
  private auth = inject(AuthService);

  devices: Device[] = [];

  ngOnInit() {
    this.loadDevices();
  }

  loadDevices() {
    this.api.getUnassignedDevices().subscribe(data => {
        this.devices = data.filter(d => !d.userId);
    });
  }

  assign(deviceId: number) {
    const currentUser = this.auth.currentUser();
    if (currentUser) {
      this.api.updateDevice(deviceId, { userId: currentUser.id }).subscribe(() => {
        this.loadDevices();
      });
    }
  }
}
