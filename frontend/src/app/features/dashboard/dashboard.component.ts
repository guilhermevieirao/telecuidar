import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { BreadcrumbComponent } from '../../shared/components/breadcrumb/breadcrumb.component';
import { NotificationsComponent } from '../notifications/notifications.component';
import { ThemeToggleComponent } from '../../shared/components/theme-toggle/theme-toggle.component';
import { MobileMenu, MenuItem } from '../../shared/components/mobile-menu/mobile-menu';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, BreadcrumbComponent, NotificationsComponent, ThemeToggleComponent, MobileMenu],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  user: any = null;
  menuItems: MenuItem[] = [];

  constructor(private router: Router) {}

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    const userStr = localStorage.getItem('user');

    if (!token || !userStr) {
      this.router.navigate(['/entrar']);
      return;
    }

    this.user = JSON.parse(userStr);
    
    // Redirecionar admin para painel administrativo
    if (this.user?.role === 3) {
      this.router.navigate(['/admin']);
      return;
    }

    this.setupMenu();
  }

  setupMenu(): void {
    this.menuItems = [
      { label: 'Dashboard', icon: '🏠', route: '/painel' },
      { label: 'Meu Perfil', icon: '👤', route: '/perfil' },
      { label: 'Arquivos', icon: '📁', route: '/arquivos' },
      { label: 'Relatórios', icon: '📊', route: '/relatorios' },
      { divider: true },
      { label: 'Sair', icon: '🚪', action: () => this.logout() }
    ];
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.router.navigate(['/entrar']);
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
}
