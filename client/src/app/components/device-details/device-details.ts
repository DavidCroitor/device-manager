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
      <div class="device-details-container">
        <div class="header-section">
          <h2>{{ device.name }} Details</h2>
          <button class="back-btn" routerLink="/">
            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16"><path fill-rule="evenodd" d="M15 8a.5.5 0 0 0-.5-.5H2.707l3.147-3.146a.5.5 0 1 0-.708-.708l-4 4a.5.5 0 0 0 0 .708l4 4a.5.5 0 0 0 .708-.708L2.707 8.5H14.5A.5.5 0 0 0 15 8z"/></svg>
            Back to List
          </button>
        </div>

        <div class="cards-grid">
          <div class="detail-card">
            <span class="label">Manufacturer</span>
            <span class="value">{{ device.manufacturer }}</span>
          </div>
          <div class="detail-card">
            <span class="label">Type</span>
            <span class="value badge">{{ device.type }}</span>
          </div>
          <div class="detail-card">
            <span class="label">Operating System</span>
            <span class="value">{{ device.os }} {{ device.osVersion }}</span>
          </div>
          <div class="detail-card">
            <span class="label">Processor</span>
            <span class="value">{{ device.processor }}</span>
          </div>
          <div class="detail-card">
            <span class="label">RAM</span>
            <span class="value">{{ device.ramGB }} GB</span>
          </div>
        </div>

        <div class="info-section">
          <div class="description-card">
            <h3>Description</h3>
            <p>{{ device.description }}</p>
          </div>

          <div class="ai-card" [class.generated]="aiDescription">
            <div class="ai-header">
              <h3>✨ AI Analysis</h3>
              @if (!aiDescription) {
                <button class="ai-btn" (click)="generateDescription()" [disabled]="isGenerating">
                  {{ isGenerating ? 'Generating...' : 'Generate Insights ✨' }}
                </button>
              }
            </div>
            @if (aiDescription) {
              <div class="ai-content animated-text">
                {{ displayedAiDescription }}
                @if (isTyping) {
                  <span class="cursor">|</span>
                }
              </div>
            }
          </div>
        </div>
      </div>
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

