import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  template: `
    <div style="padding: 20px; font-family: sans-serif;">
      <h1>Device Management System</h1>
      <hr>
      <router-outlet></router-outlet>
    </div>
  `
})
export class AppComponent { }
