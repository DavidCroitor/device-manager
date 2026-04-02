import { Routes } from '@angular/router';
import { DeviceListComponent } from './components/device-list/device-list';
import { DeviceFormComponent } from './components/device-form/device-form';
import { DeviceDetailsComponent } from './components/device-details/device-details';

export const routes: Routes = [
  { path: '', component: DeviceListComponent },
  { path: 'devices/new', component: DeviceFormComponent },
  { path: 'devices/edit/:id', component: DeviceFormComponent },
  { path: 'devices/:id', component: DeviceDetailsComponent },
  { path: '**', redirectTo: '' }
];
