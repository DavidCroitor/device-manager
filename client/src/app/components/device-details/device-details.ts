import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { ApiService } from '../../services/api';
import { AuthService } from '../../services/auth.service';
import { Device } from '../../models';
import { forkJoin, timer } from 'rxjs';

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
        <li>
          <strong>AI Description:</strong>
          @if (aiDescription) {
            <span class="animated-text">
              {{ displayedAiDescription }}
              @if (isTyping) {
                <span class="cursor">✨</span>
              }
            </span>
          } @else {
            <button (click)="generateDescription()" [disabled]="isGenerating">
              {{ isGenerating ? '✨ Generating...' : 'Generate description using AI ✨' }}
            </button>
          }
        </li>
      </ul>
      <button routerLink="/">Back to List</button>
    }
  `,
  styleUrl: './device-details.css'
})
export class DeviceDetailsComponent implements OnInit {
  private api = inject(ApiService);
  private auth = inject(AuthService);
  private route = inject(ActivatedRoute);
  device: Device | null = null;
  aiDescription: string | null = null;
  displayedAiDescription: string = '';
  isGenerating = false;
  isTyping = false;

  ngOnInit() {
    this.loadDevice();
  }

  loadDevice() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.api.getDevice(id).subscribe(data => this.device = data);
  }

  generateDescription() {
    if (!this.device || !this.device.id) return;
    this.isGenerating = true;

    forkJoin({
      response: this.api.generateDescription(this.device.id),
      delay: timer(2000)
    }).subscribe({
      next: ({ response }) => {
        this.aiDescription = response.description;
        this.isGenerating = false;
        this.isTyping = true;
        this.displayedAiDescription = '';

        let i = 0;
        const typingInterval = setInterval(() => {
          if (this.aiDescription && i < this.aiDescription.length) {
            this.displayedAiDescription += this.aiDescription.charAt(i);
            i++;
          } else {
            clearInterval(typingInterval);
            this.isTyping = false;
          }
        }, 20);
      },
      error: () => {
        this.isGenerating = false;
      }
    });
  }
}

