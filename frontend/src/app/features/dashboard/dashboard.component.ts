import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { BreadcrumbComponent } from '../../shared/components/breadcrumb/breadcrumb.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, BreadcrumbComponent],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  user: any = null;

  constructor(private router: Router) {}

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    const userStr = localStorage.getItem('user');

    if (!token || !userStr) {
      this.router.navigate(['/entrar']);
      return;
    }

    this.user = JSON.parse(userStr);
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
