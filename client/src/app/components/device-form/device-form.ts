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
    <h2>{{ isEditMode ? 'Edit' : 'Create' }} Device</h2>

    @if (errorMessage) {
      <div style="color: red; margin-bottom: 15px;">{{ errorMessage }}</div>
    }

    <form [formGroup]="deviceForm" (ngSubmit)="onSubmit()">
      <div>
        <label>Name:</label>
        <input formControlName="name">
      </div>
      <div>
        <label>Manufacturer:</label>
        <input formControlName="manufacturer">
      </div>
      <div>
        <label>Type:</label>
        <select formControlName="type">
          <option value="phone">Phone</option>
          <option value="tablet">Tablet</option>
        </select>
      </div>
      <div>
        <label>OS:</label>
        <input formControlName="os">
      </div>
      <div>
        <label>OS Version:</label>
        <input formControlName="osVersion">
      </div>
      <div>
        <label>Processor:</label>
        <input formControlName="processor">
      </div>
      <div>
        <label>RAM (GB):</label>
        <input type="number" formControlName="ramGB">
      </div>
      <div>
        <label>Description:</label>
        <textarea formControlName="description"></textarea>
      </div>

      <div style="margin-top: 15px;">
        <button type="submit" [disabled]="deviceForm.invalid">Save</button>
        <button type="button" routerLink="/">Cancel</button>
        @if (isEditMode) {
          <button type="button" (click)="unassign()" style="margin-left: 10px;" class="btn btn-unassign">Unassign</button>
        }
      </div>
    </form>
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
