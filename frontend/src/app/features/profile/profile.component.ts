import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { ToastService } from '../../core/services/toast.service';
import { NgxMaskDirective } from 'ngx-mask';
import { BreadcrumbComponent } from '../../shared/components/breadcrumb/breadcrumb.component';
import { ImageCropModalComponent } from '../../shared/components/image-crop-modal/image-crop-modal.component';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, NgxMaskDirective, BreadcrumbComponent, ImageCropModalComponent],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  profileForm: FormGroup;
  loading = false;
  user: any = null;
  profilePhotoUrl: string | null = null;
  profilePhotoPreview: string | null = null;
  showCropModal = false;
  tempImageForCrop: string = '';

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private router: Router,
    private toastService: ToastService
  ) {
    this.profileForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', [Validators.required]],
    });
  }

  ngOnInit() {
    this.loadUserData();
  }

  loadUserData() {
    const userString = localStorage.getItem('user');
    if (!userString) {
      this.router.navigate(['/entrar']);
      return;
    }

    this.user = JSON.parse(userString);
    
    this.http.get<any>(`http://localhost:5058/api/users/${this.user.id}`)
      .subscribe({
        next: (response) => {
          if (response.isSuccess) {
            const userData = response.data;
            const formattedPhone = this.formatPhone(userData.phoneNumber || '');
            this.profilePhotoUrl = userData.profilePhotoUrl;
            this.profilePhotoPreview = userData.profilePhotoUrl;
            
            this.profileForm.patchValue({
              firstName: userData.firstName,
              lastName: userData.lastName,
              email: userData.email,
              phoneNumber: formattedPhone
            });
          }
        },
        error: (error) => {
          console.error('Erro ao carregar perfil:', error);
          this.toastService.error('Erro ao carregar dados do perfil');
        }
      });
  }

  formatPhone(phone: string): string {
    if (!phone) return '';
    const cleaned = phone.replace(/\D/g, '');
    if (cleaned.length === 11) {
      return `(${cleaned.substring(0, 2)}) ${cleaned.substring(2, 7)}-${cleaned.substring(7)}`;
    } else if (cleaned.length === 10) {
      return `(${cleaned.substring(0, 2)}) ${cleaned.substring(2, 6)}-${cleaned.substring(6)}`;
    }
    return phone;
  }

  onProfilePhotoChange(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      
      // Validar tamanho (máx 5MB para permitir crop)
      if (file.size > 5 * 1024 * 1024) {
        this.toastService.error('A imagem deve ter no máximo 5MB');
        return;
      }

      // Validar tipo
      if (!file.type.startsWith('image/')) {
        this.toastService.error('Apenas imagens são permitidas');
        return;
      }

      const reader = new FileReader();
      reader.onload = () => {
        this.tempImageForCrop = reader.result as string;
        this.showCropModal = true;
      };
      reader.readAsDataURL(file);
    }
    
    // Reset input para permitir selecionar a mesma imagem novamente
    input.value = '';
  }

  onCropComplete(croppedImage: string) {
    this.profilePhotoPreview = croppedImage;
    this.profilePhotoUrl = croppedImage;
    this.showCropModal = false;
    this.toastService.success('Imagem recortada com sucesso!');
  }

  onCropCancel() {
    this.showCropModal = false;
    this.tempImageForCrop = '';
  }

  removeProfilePhoto() {
    this.profilePhotoPreview = null;
    this.profilePhotoUrl = null;
  }

  onSubmit() {
    if (this.profileForm.invalid) {
      this.toastService.warning('Por favor, preencha todos os campos corretamente');
      return;
    }

    this.loading = true;
    const formData = this.profileForm.value;
    
    const updateData = {
      id: this.user.id,
      ...formData,
      phoneNumber: formData.phoneNumber.replace(/[^\d]/g, ''),
      profilePhotoUrl: this.profilePhotoUrl
    };

    this.http.put(`http://localhost:5058/api/users/${this.user.id}`, updateData)
      .subscribe({
        next: (response: any) => {
          if (response.isSuccess) {
            // Atualizar localStorage com novos dados
            const updatedUser = {
              ...this.user,
              firstName: formData.firstName,
              lastName: formData.lastName,
              fullName: `${formData.firstName} ${formData.lastName}`,
              email: formData.email,
              profilePhotoUrl: this.profilePhotoUrl
            };
            localStorage.setItem('user', JSON.stringify(updatedUser));
            
            this.toastService.success('Perfil atualizado com sucesso!');
            this.loading = false;
          }
        },
        error: (error) => {
          console.error('Erro ao atualizar perfil:', error);
          this.toastService.error(error.error?.message || 'Erro ao atualizar perfil');
          this.loading = false;
        }
      });
  }

  get firstName() { return this.profileForm.get('firstName'); }
  get lastName() { return this.profileForm.get('lastName'); }
  get email() { return this.profileForm.get('email'); }
  get phoneNumber() { return this.profileForm.get('phoneNumber'); }
}
