import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { NgxMaskDirective } from 'ngx-mask';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, NgxMaskDirective],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  registerForm: FormGroup;
  loading = false;
  errorMessage = '';
  successMessage = '';
  showPassword = false;
  showConfirmPassword = false;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private router: Router,
    private toastService: ToastService
  ) {
    this.registerForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]],
      role: [1], // 1 = Paciente (default)
      acceptTerms: [false, [Validators.requiredTrue]]
    }, { validators: this.passwordMatchValidator });
  }

  passwordMatchValidator(group: FormGroup) {
    const password = group.get('password')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;
    return password === confirmPassword ? null : { passwordMismatch: true };
  }

  onSubmit(): void {
    if (this.registerForm.valid) {
      this.loading = true;
      this.errorMessage = '';
      this.successMessage = '';

      const { confirmPassword, acceptTerms, ...formData } = this.registerForm.value;
      
      // Remove máscara do telefone antes de enviar
      const data = {
        ...formData,
        phoneNumber: formData.phoneNumber.replace(/[^\d]/g, '')
      };

      this.http.post<any>('http://localhost:5058/api/auth/register', data)
        .subscribe({
          next: (response) => {
            if (response.isSuccess) {
              this.toastService.success('Cadastro realizado! Verifique seu email para confirmar a conta.');
              this.successMessage = 'Cadastro realizado! Verifique seu email para confirmar sua conta.';
              setTimeout(() => {
                this.router.navigate(['/login']);
              }, 3000);
            } else {
              this.errorMessage = response.message || 'Erro ao cadastrar';
              this.toastService.error(this.errorMessage);
            }
            this.loading = false;
          },
          error: (error) => {
            this.errorMessage = error.error?.message || 'Erro ao realizar cadastro. Tente novamente.';
            this.toastService.error(this.errorMessage);
            this.loading = false;
          }
        });
    }
  }

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  toggleConfirmPassword(): void {
    this.showConfirmPassword = !this.showConfirmPassword;
  }

  get firstName() {
    return this.registerForm.get('firstName');
  }

  get lastName() {
    return this.registerForm.get('lastName');
  }

  get email() {
    return this.registerForm.get('email');
  }

  get phoneNumber() {
    return this.registerForm.get('phoneNumber');
  }

  get password() {
    return this.registerForm.get('password');
  }

  get confirmPassword() {
    return this.registerForm.get('confirmPassword');
  }

  get acceptTerms() {
    return this.registerForm.get('acceptTerms');
  }
}
