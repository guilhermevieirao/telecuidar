import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ButtonComponent } from '@shared/components/atoms/button/button';
import { IconComponent } from '@shared/components/atoms/icon/icon';
import { ThemeToggleComponent } from '@shared/components/atoms/theme-toggle/theme-toggle';
import { ModalService } from '@core/services/modal.service';

@Component({
  selector: 'app-mobile-upload',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonComponent, IconComponent, ThemeToggleComponent],
  templateUrl: './mobile-upload.html',
  styleUrls: ['./mobile-upload.scss']
})
export class MobileUploadComponent implements OnInit {
  title = '';
  selectedFile: File | null = null;
  previewUrl: string | null = null;
  isSubmitting = false;
  isSuccess = false;
  token: string | null = null;

  constructor(
    private modalService: ModalService,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.token = this.route.snapshot.paramMap.get('token');
  }

  onFileSelected(event: any) {
    // Deprecated, replaced by specific methods
    this.handleFile(event.target.files[0]);
  }

  onSubmit() {
    if (!this.title || !this.selectedFile || !this.previewUrl || !this.token) return;

    this.isSubmitting = true;
    
    // Simulate network delay
    setTimeout(() => {
      // Save to localStorage to simulate real-time transfer
      // The desktop component will be polling for this key
      const payload = {
        title: this.title,
        fileUrl: this.previewUrl,
        type: this.selectedFile?.type.startsWith('image/') ? 'image' : 'document',
        timestamp: new Date().getTime()
      };

      localStorage.setItem(`mobile_upload_${this.token}`, JSON.stringify(payload));

      this.isSubmitting = false;
      this.isSuccess = true;
    }, 1500);
  }

  // File Inputs
  onTakePhoto(event: any) {
    this.handleFile(event.target.files[0]);
  }

  onChoosePhoto(event: any) {
    this.handleFile(event.target.files[0]);
  }

  onChooseFile(event: any) {
    this.handleFile(event.target.files[0]);
  }

  private handleFile(file: File) {
    if (file) {
      this.selectedFile = file;
      
      // For images, generate preview
      if (file.type.startsWith('image/')) {
        const reader = new FileReader();
        reader.onload = (e: any) => this.previewUrl = e.target.result;
        reader.readAsDataURL(file);
      } else {
        // For docs, use a placeholder icon/name
        this.previewUrl = null; // No image preview
      }
    }
  }
}
