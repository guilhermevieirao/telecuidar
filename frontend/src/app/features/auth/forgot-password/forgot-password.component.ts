import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss']
})
export class ForgotPasswordComponent {
  forgotPasswordForm: FormGroup;
  loading = false;
  errorMessage = '';
  successMessage = '';
  emailSent = false;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private toastService: ToastService
  ) {
    this.forgotPasswordForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });
  }

  onSubmit(): void {
    if (this.forgotPasswordForm.valid) {
      this.loading = true;
      this.errorMessage = '';
      this.successMessage = '';

      this.http.post<any>('http://localhost:5058/api/auth/request-password-reset', this.forgotPasswordForm.value)
        .subscribe({
          next: (response) => {
            if (response.isSuccess) {
              this.emailSent = true;
              this.successMessage = 'Se o e-mail existir, você receberá instruções para redefinir sua senha.';
            } else {
              this.errorMessage = response.message || 'Erro ao enviar e-mail';
            }
            this.loading = false;
          },
          error: (error) => {
            this.errorMessage = error.error?.message || 'Erro ao processar solicitação. Tente novamente.';
            this.loading = false;
          }
        });
    }
  }

  get email() {
    return this.forgotPasswordForm.get('email');
  }
}
