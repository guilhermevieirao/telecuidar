import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent implements OnInit {
  resetForm: FormGroup;
  token: string = '';
  loading = false;
  success = false;
  error: string | null = null;
  showPassword = false;
  showConfirmPassword = false;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private router: Router,
    private route: ActivatedRoute,
    private toastService: ToastService
  ) {
    this.resetForm = this.fb.group({
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });
  }

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.token = params['token'];
      if (!this.token) {
        this.error = 'Token de redefinição inválido ou expirado.';
      }
    });
  }

  passwordMatchValidator(g: FormGroup) {
    const password = g.get('password')?.value;
    const confirmPassword = g.get('confirmPassword')?.value;
    return password === confirmPassword ? null : { mismatch: true };
  }

  get passwordsMatch(): boolean {
    return !this.resetForm.hasError('mismatch');
  }

  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }

  toggleConfirmPasswordVisibility() {
    this.showConfirmPassword = !this.showConfirmPassword;
  }

  onSubmit() {
    if (this.resetForm.invalid || !this.token) {
      return;
    }

    this.loading = true;
    this.error = null;

    const payload = {
      token: this.token,
      newPassword: this.resetForm.get('password')?.value
    };

    this.http.post<any>(`${environment.apiUrl}/auth/reset-password`, payload).subscribe({
      next: (response) => {
        if (response.isSuccess) {
          this.success = true;
          this.loading = false;
          this.toastService.success('Senha redefinida com sucesso! Redirecionando...');
          setTimeout(() => {
            this.router.navigate(['/entrar']);
          }, 3000);
        } else {
          this.error = response.message || 'Erro ao redefinir senha.';
          this.loading = false;
        }
      },
      error: (err: any) => {
        this.loading = false;
        this.error = err.error?.message || 'Erro ao redefinir senha. O token pode estar expirado.';
        if (this.error) {
          this.toastService.error(this.error);
        }
      }
    });
  }

  goToLogin() {
    this.router.navigate(['/entrar']);
  }
}
