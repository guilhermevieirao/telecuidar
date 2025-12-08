
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { ScheduleBlocksService, RequestBlockPayload } from './schedule-blocks.service';
import { BreadcrumbComponent } from '../../shared/components/molecules/breadcrumb/breadcrumb.component';
import { DashboardNavbarComponent } from '../../shared/components/organisms/dashboard-navbar/dashboard-navbar.component';

@Component({
  selector: 'app-request-block',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, BreadcrumbComponent, DashboardNavbarComponent],
  templateUrl: './request-block.component.html',
  styleUrls: ['./request-block.component.scss']
})
export class RequestBlockComponent {
  type: 'single' | 'range' = 'single';
  startDate = '';
  endDate = '';
  reason = '';
  loading = false;
  error = '';
  success = '';
  user: any = null;
  menuItems: any[] = [];

  constructor(private blocksService: ScheduleBlocksService, private router: Router) {
    const userStr = localStorage.getItem('user');
    if (userStr) {
      this.user = JSON.parse(userStr);
      this.setupMenu();
    }
  }

  setupMenu(): void {
    this.menuItems = [
      { label: 'Dashboard', icon: '🏠', route: '/painel' },
      { label: 'Minhas Consultas', icon: '📋', route: '/consultas-profissional' },
      { label: 'Meu Perfil', icon: '👤', route: '/perfil' },
      { divider: true },
      { label: 'Sair', icon: '🚪', action: () => this.logout() }
    ];
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    window.location.href = '/entrar';
  }

  getUserRoleName(): string {
    switch (this.user?.role) {
      case 1: return 'Paciente';
      case 2: return 'Profissional';
      default: return 'Usuário';
    }
  }

  getUserInitials(): string {
    if (!this.user?.firstName || !this.user?.lastName) return '??';
    return `${this.user.firstName.charAt(0)}${this.user.lastName.charAt(0)}`.toUpperCase();
  }

  submit() {
    this.error = '';
    this.success = '';
    if (!this.startDate || (this.type === 'range' && !this.endDate) || !this.reason) {
      this.error = 'Preencha todos os campos.';
      return;
    }
    const payload: RequestBlockPayload = {
      startDate: this.startDate,
      endDate: this.type === 'single' ? this.startDate : this.endDate,
      reason: this.reason
    };
    this.loading = true;
    this.blocksService.requestBlock(payload).subscribe({
      next: () => {
        this.success = 'Solicitação enviada!';
        setTimeout(() => this.router.navigate(['/meus-bloqueios']), 1200);
      },
      error: (err) => {
        this.error = err?.error?.message || 'Erro ao solicitar bloqueio.';
        this.loading = false;
      }
    });
  }
}
