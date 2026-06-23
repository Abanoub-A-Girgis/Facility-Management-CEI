import { HttpEventType } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ModelsService } from '../../core/services/models.service';

@Component({
  selector: 'app-model-upload',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './model-upload.component.html',
  styleUrl: './model-upload.component.css'
})
export class ModelUploadComponent {
  selectedFile?: File;
  progress = 0;
  loading = false;
  error = '';
  success = '';

  constructor(
    private readonly modelsService: ModelsService,
    private readonly router: Router
  ) {}

  onFileSelected(event: Event): void {
    this.error = '';
    this.success = '';
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) {
      this.selectedFile = undefined;
      return;
    }

    if (!file.name.toLowerCase().endsWith('.ifc')) {
      this.error = 'Only IFC files are supported.';
      this.selectedFile = undefined;
      return;
    }

    this.selectedFile = file;
  }

  upload(): void {
    if (!this.selectedFile) {
      this.error = 'Select an IFC file before uploading.';
      return;
    }

    this.loading = true;
    this.progress = 0;
    this.error = '';
    this.success = '';

    this.modelsService.upload(this.selectedFile).subscribe({
      next: event => {
        if (event.type === HttpEventType.UploadProgress && event.total) {
          this.progress = Math.round((event.loaded / event.total) * 100);
        }

        if (event.type === HttpEventType.Response) {
          const upload = event.body?.data;
          this.success = event.body?.message ?? 'Model uploaded successfully.';
          this.loading = false;
          if (upload?.modelId) {
            this.router.navigate(['/models', upload.modelId, 'viewer']);
          }
        }
      },
      error: err => {
        this.loading = false;
        this.error = err.error?.message ?? 'Upload failed. Check the file and try again.';
      }
    });
  }
}
