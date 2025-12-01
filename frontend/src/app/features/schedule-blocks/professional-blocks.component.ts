
import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ScheduleBlocksService, ScheduleBlock } from './schedule-blocks.service';

import { BreadcrumbComponent } from '../../shared/components/breadcrumb/breadcrumb.component';
import { MenuItem } from '../../shared/components/mobile-menu/mobile-menu';
import { DashboardNavbarComponent } from '../../shared/components/dashboard-navbar/dashboard-navbar.component';

@Component({
  selector: 'app-professional-blocks',
  standalone: true,
  imports: [CommonModule, DatePipe, RouterModule, BreadcrumbComponent, DashboardNavbarComponent],
  templateUrl: './professional-blocks.component.html',
  styleUrls: ['./professional-blocks.component.scss']
})
export class ProfessionalBlocksComponent implements OnInit {
  blocks: ScheduleBlock[] = [];
  loading = false;
  user: any = null;
  menuItems: MenuItem[] = [];

  constructor(private blocksService: ScheduleBlocksService) {}

  ngOnInit(): void {
    this.fetchBlocks();
    const userStr = localStorage.getItem('user');
    if (userStr) {
      this.user = JSON.parse(userStr);
      this.setupMenu();
    }
  }

  fetchBlocks() {
    this.loading = true;
    this.blocksService.getMyBlocks().subscribe({
      next: (blocks) => {
        this.blocks = blocks;
        this.loading = false;
      },
      error: () => { this.loading = false; }
    });
  }

  getStatusLabel(block: ScheduleBlock): string {
    // status pode vir como número (enum) ou string
    const statusMap: Record<string | number, string> = {
      0: 'Pendente',
      1: 'Aceita',
      2: 'Recusada',
      3: 'Passada',
      'Pending': 'Pendente',
      'Accepted': 'Aceita',
      'Rejected': 'Recusada',
      'Expired': 'Passada',
      'Pendente': 'Pendente',
      'Aceita': 'Aceita',
      'Recusada': 'Recusada',
      'Passada': 'Passada',
    };
    return statusMap[block.status] || block.status;
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
}
