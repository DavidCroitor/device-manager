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
    <div class="pagination">
      <button (click)="prevPage()" [disabled]="pageNumber === 1">Previous</button>
      <span>Page {{ pageNumber }}</span>
      <button (click)="nextPage()" [disabled]="devices.length < pageSize">Next</button>
    </div>
  `,
  styleUrl: '../device-list.css'
})
export class AllDevicesComponent implements OnInit {
  private api = inject(ApiService);
  devices: Device[] = [];
  isSearching = false;
  currentQuery = '';
  pageNumber = 1;
  pageSize = 10;

  ngOnInit() {
    this.loadDevices();
  }

  loadDevices() {
    this.api.getDevices(this.pageNumber, this.pageSize).subscribe(data => this.devices = data);
  }

    onSearch(query: string, resetPage = true) {
    if (resetPage) {
      this.pageNumber = 1;
    }
    this.currentQuery = query;
    if (!query.trim()) {
      this.clearSearch();
      return;
    }
    this.isSearching = true;

    this.api.searchDevices(query, this.pageNumber, this.pageSize).subscribe(data => this.devices = data);
  }

  clearSearch() {
    this.currentQuery = '';
    this.isSearching = false;
    this.pageNumber = 1;
    this.loadDevices();
  }

  prevPage() {
    if (this.pageNumber > 1) {
      this.pageNumber--;
      this.isSearching ? this.onSearch(this.currentQuery, false) : this.loadDevices();
    }
  }

  nextPage() {
    if (this.devices.length >= this.pageSize) {
      this.pageNumber++;
      this.isSearching ? this.onSearch(this.currentQuery, false) : this.loadDevices();
    }
  }
}
