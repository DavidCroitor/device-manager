import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { ApiService } from '../../services/api';
import { Device } from '../../models';

@Component({
  selector: 'app-device-details',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    @if (device) {
      <h2>{{ device.name }} Details</h2>
      <ul>
        <li><strong>Manufacturer:</strong> {{ device.manufacturer }}</li>
        <li><strong>Type:</strong> {{ device.type }}</li>
        <li><strong>OS:</strong> {{ device.os }} {{ device.osVersion }}</li>
        <li><strong>Processor:</strong> {{ device.processor }}</li>
        <li><strong>RAM:</strong> {{ device.ramGB }} GB</li>
        <li><strong>Description:</strong> {{ device.description }}</li>
      </ul>
      <h3>Assigned To</h3>
      <p>{{ device.userName }} ({{ device.userRole }}) - {{ device.userLocation }}</p>

      <button routerLink="/">Back to List</button>
    }
  `,
  styleUrl: './device-details.css'
})
export class DeviceDetailsComponent implements OnInit {
  private api = inject(ApiService);
  private route = inject(ActivatedRoute);
  device: Device | null = null;

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.api.getDevice(id).subscribe(data => this.device = data);
  }
}
