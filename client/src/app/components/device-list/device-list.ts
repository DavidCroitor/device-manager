import { Component, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AllDevicesComponent } from './all-devices/all-devices';
import { MyDevicesComponent } from './my-devices/my-devices';
import { UnassignedDevicesComponent } from './unassigned-devices/unassigned-devices';

@Component({
  selector: 'app-device-list',
  standalone: true,
  imports: [CommonModule, RouterModule, AllDevicesComponent, MyDevicesComponent, UnassignedDevicesComponent],
  template: `
    <h2>Devices</h2>

    <div class="tabs">
      <button [class.active]="currentList === 'all'" (click)="setList('all')" class="btn">All Devices</button>
      <button [class.active]="currentList === 'mine'" (click)="setList('mine')" class="btn">My Devices</button>
      <button [class.active]="currentList === 'unassigned'" (click)="setList('unassigned')" class="btn">Unassigned Devices</button>
    </div>

    <div class="actions-container" style="display: flex; justify-content: space-between; align-items: center; max-width: 80%; margin: 0 auto 16px auto;">
      @if (currentList === 'all') {
        <div class="search-container">
          <input type="text" #searchInput class="search-input" placeholder="Search devices..." (keyup.enter)="onSearch(searchInput.value)">
          <button class="btn btn-search" (click)="onSearch(searchInput.value)">Search</button>
          @if (isSearching) {
            <button class="btn btn-secondary" (click)="clearSearch(searchInput)">Clear</button>
          }
        </div>
      } @else {
        <div></div> <!-- Spacer -->
      }
      <button routerLink="/devices/new" class="btn btn-create" style="margin-bottom: 0;">Create New Device</button>
    </div>

    @if (currentList === 'all') {
      <app-all-devices #allDevices></app-all-devices>
    } @else if (currentList === 'mine') {
      <app-my-devices></app-my-devices>
    } @else if (currentList === 'unassigned') {
      <app-unassigned-devices></app-unassigned-devices>
    }


    `,
  styleUrl: './device-list.css'
})
export class DeviceListComponent {
  currentList: 'all' | 'mine' | 'unassigned' = 'all';

  @ViewChild('allDevices') allDevicesCmp?: AllDevicesComponent;

  setList(list: 'all' | 'mine' | 'unassigned') {
    this.currentList = list;
  }

  onSearch(query: string) {
    if (this.allDevicesCmp) {
      this.allDevicesCmp.onSearch(query);
    }
  }

  clearSearch(input: HTMLInputElement) {
    input.value = '';
    if (this.allDevicesCmp) {
      this.allDevicesCmp.clearSearch();
    }
  }

  get isSearching() {
    return this.allDevicesCmp?.isSearching || false;
  }
}

