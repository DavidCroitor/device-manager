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
          <tr><td colspan="4">No unassigned devices found.</td></tr>
        }
      </tbody>
    </table>
    <div class="pagination">
      <button (click)="prevPage()" [disabled]="pageNumber === 1">Previous</button>
      <span>Page {{ pageNumber }}</span>
      <button (click)="nextPage()" [disabled]="devices.length < pageSize">Next</button>
    </div>
  `,
  styleUrl: '../device-list.css'
})
export class UnassignedDevicesComponent implements OnInit {
  private api = inject(ApiService);
  private auth = inject(AuthService);

  devices: Device[] = [];
  pageNumber = 1;
  pageSize = 10;

  ngOnInit() {
    this.loadDevices();
  }

  loadDevices() {
    this.api.getUnassignedDevices(this.pageNumber, this.pageSize).subscribe(data => {
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

  prevPage() {
    if (this.pageNumber > 1) {
      this.pageNumber--;
      this.loadDevices();
    }
  }

  nextPage() {
    if (this.devices.length >= this.pageSize) {
      this.pageNumber++;
      this.loadDevices();
    }
  }
}
