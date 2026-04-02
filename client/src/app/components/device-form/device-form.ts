import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ApiService } from '../../services/api';
import { User } from '../../models';

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
        <input formControlName="type">
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

      <!-- UserId is only needed for creation based on your UpdateDeviceRequestDto -->
      @if (!isEditMode) {
        <div>
          <label>Assign to User:</label>
          <select formControlName="userId">
            <option value="" disabled>Select User</option>
            @for (user of users; track user.id) {
              <option [value]="user.id">{{ user.name }}</option>
            }
          </select>
        </div>
      }

      <div style="margin-top: 15px;">
        <button type="submit" [disabled]="deviceForm.invalid">Save</button>
        <button type="button" routerLink="/">Cancel</button>
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

  users: User[] = [];
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
    description: ['', Validators.required],
    userId: [null as number | null ] // Required dynamically if in create mode
  });

  ngOnInit() {
    this.deviceId = Number(this.route.snapshot.paramMap.get('id'));
    this.isEditMode = !!this.deviceId;

    if (!this.isEditMode) {
      // Require userId for creation
      this.deviceForm.get('userId')?.setValidators(Validators.required);
      this.api.getUsers().subscribe(u => this.users = u);
    } else {
      // Load device details to populate form for updating
      this.api.getDevice(this.deviceId).subscribe(device => {
        this.deviceForm.patchValue(device);
      });
    }
  }

  onSubmit() {
    if (this.deviceForm.invalid) return;
    this.errorMessage = '';

    if (this.isEditMode) {
      // Remove userId from payload since UpdateDeviceRequestDto doesn't expect it
      const { userId, ...updatePayload } = this.deviceForm.value;

      this.api.updateDevice(this.deviceId!, updatePayload).subscribe({
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

    private handleError(err: any) {
    if (err.status === 409) {
      this.errorMessage = 'This device already exists for this user. Please choose a different name/configuration or choose a different user.';
    } else if (err.status === 400) {
      // Check if the API returned a structured ProblemDetails error object
      if (err.error && err.error.errors) {
        const errorDict = err.error.errors;

        // Map through all keys to extract the specific validation messages
        const detailedMessages = Object.keys(errorDict)
          .map(key => `${errorDict[key].join(' ')}`)
          .join('\n');

        this.errorMessage = `Validation error:\n${detailedMessages}`;
      } else {
        // Fallback for an unstructured 400 error
        this.errorMessage = 'Validation error. Please check your inputs.';
      }
    } else {
      this.errorMessage = 'An unexpected error occurred.';
    }
  }
}
