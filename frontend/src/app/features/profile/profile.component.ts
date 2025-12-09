import { Component, OnInit, Input } from '@angular/core';
import { CommonModule, NgIf, NgFor } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { ToastService } from '../../core/services/toast.service';
import { environment } from '../../../environments/environment';
import { NgxMaskDirective } from 'ngx-mask';
import { ImageCropModalComponent } from '../../shared/components/organisms/image-crop-modal/image-crop-modal.component';
import { NotificationsComponent } from '../notifications/notifications.component';
import { ButtonComponent } from '../../shared/components/atoms/button/button.component';
import { BadgeComponent } from '../../shared/components/atoms/badge/badge.component';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, NgIf, ReactiveFormsModule, RouterLink, NgxMaskDirective, ImageCropModalComponent, NotificationsComponent, ButtonComponent, BadgeComponent],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  @Input() embeddedMode = false; // Quando true, oculta header
  
  profileForm: FormGroup;
  changePasswordForm: FormGroup;
  loading = false;
  isEditMode = false; // Controla se está no modo de edição
  user: any = null;
  profilePhotoUrl: string | null = null;
  profilePhotoPreview: string | null = null;
  showCropModal = false;
  tempImageForCrop: string = '';
  showDeleteModal = false;
  showChangePasswordModal = false;
  showCurrentPassword = false;
  showNewPassword = false;
  showConfirmPassword = false;

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

    this.changePasswordForm = this.fb.group({
      currentPassword: ['', [Validators.required]],
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmNewPassword: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });
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
    
    this.http.get<any>(`${environment.apiUrl}/users/${this.user.id}`)
      .subscribe({
        next: (response) => {
          if (response.isSuccess) {
            const userData = response.data;
            const formattedPhone = this.formatPhone(userData.phoneNumber || '');
            this.profilePhotoUrl = userData.profilePhotoUrl;
            this.profilePhotoPreview = userData.profilePhotoUrl;
            
            // Atualiza o objeto user com todas as informações
            this.user = {
              ...this.user,
              ...userData
            };
            
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

  enableEditMode() {
    this.isEditMode = true;
  }

  cancelEdit() {
    this.isEditMode = false;
    this.loadUserData(); // Recarrega os dados originais
    this.toastService.info('Edição cancelada');
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

    this.http.put(`${environment.apiUrl}/users/${this.user.id}`, updateData)
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
            this.isEditMode = false; // Volta para modo visualização
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
  get currentPassword() { return this.changePasswordForm.get('currentPassword'); }
  get newPassword() { return this.changePasswordForm.get('newPassword'); }
  get confirmNewPassword() { return this.changePasswordForm.get('confirmNewPassword'); }

  get passwordsMatch(): boolean {
    return !this.changePasswordForm.hasError('mismatch');
  }

  passwordMatchValidator(g: FormGroup) {
    const newPassword = g.get('newPassword')?.value;
    const confirmNewPassword = g.get('confirmNewPassword')?.value;
    return newPassword === confirmNewPassword ? null : { mismatch: true };
  }

  openChangePasswordModal() {
    this.changePasswordForm.reset();
    this.showCurrentPassword = false;
    this.showNewPassword = false;
    this.showConfirmPassword = false;
    this.showChangePasswordModal = true;
  }

  closeChangePasswordModal() {
    this.showChangePasswordModal = false;
    this.changePasswordForm.reset();
  }

  confirmChangePassword() {
    if (this.changePasswordForm.invalid) {
      this.toastService.warning('Por favor, preencha todos os campos corretamente');
      return;
    }

    this.loading = true;
    const payload = {
      userId: this.user.id,
      currentPassword: this.changePasswordForm.get('currentPassword')?.value,
      newPassword: this.changePasswordForm.get('newPassword')?.value
    };

    this.http.post(`${environment.apiUrl}/users/change-password`, payload).subscribe({
      next: (response: any) => {
        if (response.isSuccess) {
          this.toastService.success('Senha alterada com sucesso!');
          this.closeChangePasswordModal();
          this.loading = false;
        } else {
          this.toastService.error(response.message || 'Erro ao alterar senha');
          this.loading = false;
        }
      },
      error: (error) => {
        console.error('Erro ao alterar senha:', error);
        this.toastService.error(error.error?.message || 'Erro ao alterar senha. Verifique sua senha atual.');
        this.loading = false;
      }
    });
  }

  openDeleteAccountModal() {
    this.showDeleteModal = true;
  }

  closeDeleteModal() {
    this.showDeleteModal = false;
  }

  confirmDeleteAccount() {
    if (!this.user?.id) return;

    this.loading = true;
    this.http.delete(`${environment.apiUrl}/users/account`, {
      body: { userId: this.user.id }
    }).subscribe({
      next: () => {
        this.toastService.success('Conta excluída com sucesso. Seus dados foram anonimizados conforme a LGPD.');
        localStorage.clear();
        setTimeout(() => {
          this.router.navigate(['/']);
        }, 2000);
      },
      error: (error) => {
        console.error('Erro ao excluir conta:', error);
        this.toastService.error('Erro ao excluir conta. Tente novamente.');
        this.loading = false;
        this.closeDeleteModal();
      }
    });
  }
}
