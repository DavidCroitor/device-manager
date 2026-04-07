import { Routes } from '@angular/router';
import { DeviceListComponent } from './components/device-list/device-list';
import { DeviceFormComponent } from './components/device-form/device-form';
import { DeviceDetailsComponent } from './components/device-details/device-details';
import { LoginComponent } from './components/auth/login/login';
import { RegisterComponent } from './components/auth/register/register';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: '', component: DeviceListComponent, canActivate: [authGuard] },
  { path: 'devices/new', component: DeviceFormComponent, canActivate: [authGuard] },
  { path: 'devices/edit/:id', component: DeviceFormComponent, canActivate: [authGuard] },
  { path: 'devices/:id', component: DeviceDetailsComponent, canActivate: [authGuard] },
  { path: '**', redirectTo: '' }
];
