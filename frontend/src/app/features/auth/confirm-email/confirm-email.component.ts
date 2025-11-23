import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-confirm-email',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.scss']
})
export class ConfirmEmailComponent implements OnInit {
  loading = true;
  success = false;
  errorMessage = '';
  token = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private http: HttpClient
  ) {}

  ngOnInit(): void {
    this.token = this.route.snapshot.queryParams['token'] || '';
    
    if (!this.token) {
      this.loading = false;
      this.errorMessage = 'Token de confirmação inválido';
      return;
    }

    this.confirmEmail();
  }

  confirmEmail(): void {
    this.http.post<any>('http://localhost:5058/api/auth/confirm-email', { token: this.token })
      .subscribe({
        next: (response) => {
          this.loading = false;
          if (response.isSuccess) {
            this.success = true;
            setTimeout(() => {
              this.router.navigate(['/entrar']);
            }, 3000);
          } else {
            this.errorMessage = response.message || 'Erro ao confirmar email';
          }
        },
        error: (error) => {
          this.loading = false;
          this.errorMessage = error.error?.message || 'Erro ao confirmar email. Tente novamente.';
        }
      });
  }
}
