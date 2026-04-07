import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ApiService } from '../../services/api';

@Component({
  selector: 'app-device-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  template: `
    <div class="form-container">
      <div class="header-section">
        <h2>{{ isEditMode ? 'Edit' : 'Create' }} Device</h2>
        <p class="subtitle">{{ isEditMode ? 'Update the details for this device.' : 'Add a new device to the registry.' }}</p>
      </div>

      @if (errorMessage) {
        <div class="error-banner">
          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
            <path d="M8.982 1.566a1.13 1.13 0 0 0-1.96 0L.165 13.233c-.457.778.091 1.767.98 1.767h13.713c.889 0 1.438-.99.98-1.767L8.982 1.566zM8 5c.535 0 .954.462.9.995l-.35 3.507a.552.552 0 0 1-1.1 0L7.1 5.995A.905.905 0 0 1 8 5zm.002 6a1 1 0 1 1 0 2 1 1 0 0 1 0-2z"/>
          </svg>
          <span>{{ errorMessage }}</span>
        </div>
      }

      <form [formGroup]="deviceForm" (ngSubmit)="onSubmit()" class="modern-form">
        <div class="form-grid">
          <div class="form-group full-width">
            <label>Name</label>
            <input formControlName="name" placeholder="e.g. iPhone 13 Pro">
          </div>

          <div class="form-group">
            <label>Manufacturer</label>
            <input formControlName="manufacturer" placeholder="e.g. Apple">
          </div>

          <div class="form-group">
            <label>Type</label>
            <select formControlName="type">
              <option value="" disabled selected>Select type...</option>
              <option value="phone">Phone</option>
              <option value="tablet">Tablet</option>
            </select>
          </div>

          <div class="form-group">
            <label>Operating System</label>
            <input formControlName="os" placeholder="e.g. iOS">
          </div>

          <div class="form-group">
            <label>OS Version</label>
            <input formControlName="osVersion" placeholder="e.g. 15.0">
          </div>

          <div class="form-group">
            <label>Processor</label>
            <input formControlName="processor" placeholder="e.g. A15 Bionic">
          </div>

          <div class="form-group">
            <label>RAM (GB)</label>
            <input type="number" formControlName="ramGB" placeholder="e.g. 6">
          </div>

          <div class="form-group full-width">
            <label>Description</label>
            <textarea formControlName="description" placeholder="Add any additional details or notes about this device..."></textarea>
          </div>
        </div>

        <div class="form-actions">
          <button type="submit" class="btn-primary" [disabled]="deviceForm.invalid">
            {{ isEditMode ? 'Save Changes' : 'Create Device' }}
          </button>
          <button type="button" class="btn-secondary" routerLink="/">Cancel</button>

          @if (isEditMode) {
            <div class="action-divider"></div>
            <button type="button" (click)="unassign()" class="btn-unassign">
               Unassign User
            </button>
          }
        </div>
      </form>
    </div>
  `,
  styleUrl: './device-form.css'
})
export class DeviceFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private api = inject(ApiService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  isEditMode = false;
  deviceId: number | null = null;
  errorMessage = '';

  deviceForm = this.fb.group({
    name: ['', Validators.required],
    manufacturer: ['', Validators.required],
    type: ['', Validators.required],
    os: ['', Validators.required],
    osVersion: ['', Validators.required],
    processor: ['', Validators.required],
    ramGB: [null as number | null, [Validators.required, Validators.min(1)]],
    description: ['', Validators.required]
  });

  ngOnInit() {
    this.deviceId = Number(this.route.snapshot.paramMap.get('id'));
    this.isEditMode = !!this.deviceId;

    if (this.isEditMode) {
      this.api.getDevice(this.deviceId).subscribe(device => {
        this.deviceForm.patchValue(device);
      });
    }
  }

  onSubmit() {
    if (this.deviceForm.invalid) return;
    this.errorMessage = '';

    if (this.isEditMode) {
      this.api.updateDevice(this.deviceId!, this.deviceForm.value).subscribe({
        next: () => this.router.navigate(['/']),
        error: (err) => this.handleError(err)
      });
    } else {
      this.api.createDevice(this.deviceForm.value).subscribe({
        next: () => this.router.navigate(['/']),
        error: (err) => this.handleError(err)
      });
    }
  }

  unassign() {
    if (this.deviceId) {
      this.api.updateDevice(this.deviceId, { userId: 0 }).subscribe({
        next: () => this.router.navigate(['/']),
        error: (err) => this.handleError(err)
      });
    }
  }

  private handleError(err: any) {
    if (err.status === 409) {
      this.errorMessage = 'This device already exists for this user. Please choose a different name/configuration or choose a different user.';
    } else if (err.status === 400) {
      if (err.error && err.error.errors) {
        const errorDict = err.error.errors;

        const detailedMessages = Object.keys(errorDict)
          .map(key => `${errorDict[key].join(' ')}`)
          .join('\n');

        this.errorMessage = `Validation error:\n${detailedMessages}`;
      } else {
        this.errorMessage = 'Validation error. Please check your inputs.';
      }
    } else {
      this.errorMessage = 'An unexpected error occurred.';
    }
  }
}
