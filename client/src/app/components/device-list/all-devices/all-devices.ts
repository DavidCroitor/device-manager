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
            </td>
          </tr>
        } @empty {
          <tr><td colspan="4">No devices found.</td></tr>
        }
      </tbody>
    </table>
  `,
  styleUrl: '../device-list.css'
})
export class AllDevicesComponent implements OnInit {
  private api = inject(ApiService);
  devices: Device[] = [];
  isSearching = false;

  ngOnInit() {
    this.loadDevices();
  }

  loadDevices() {
    this.api.getDevices().subscribe(data => this.devices = data);
  }

  onSearch(query: string) {
    if (!query.trim()) {
      this.clearSearch();
      return;
    }
    this.isSearching = true;
    this.api.searchDevices(query).subscribe(data => this.devices = data);
  }

  clearSearch() {
    this.isSearching = false;
    this.loadDevices();
  }
}
