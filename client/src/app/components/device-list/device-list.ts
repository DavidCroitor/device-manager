import { Component } from '@angular/core';
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

    @if (currentList === 'all') {
      <app-all-devices></app-all-devices>
    } @else if (currentList === 'mine') {
      <app-my-devices></app-my-devices>
    } @else if (currentList === 'unassigned') {
      <app-unassigned-devices></app-unassigned-devices>
    }

    <div class="actions-container">
      <button routerLink="/devices/new" class="btn btn-create">Create New Device</button>
    </div>
    `,
  styleUrl: './device-list.css'
})
export class DeviceListComponent {
  currentList: 'all' | 'mine' | 'unassigned' = 'all';

  setList(list: 'all' | 'mine' | 'unassigned') {
    this.currentList = list;
  }
}

