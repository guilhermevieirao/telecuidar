import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SpinnerComponent } from '../../shared/components/spinner/spinner.component';
import { ToastService } from '../../core/services/toast.service';
import { BreadcrumbComponent } from '../../shared/components/breadcrumb/breadcrumb.component';

interface AuditLog {
  id: string;
  userId: string;
  user: {
    firstName: string;
    lastName: string;
    email: string;
  };
  action: string;
  details: string;
  ipAddress: string;
  timestamp: string;
}

@Component({
  selector: 'app-audit-logs',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, SpinnerComponent, BreadcrumbComponent],
  templateUrl: './audit-logs.component.html',
  styleUrls: ['./audit-logs.component.scss']
})
export class AuditLogsComponent implements OnInit {
  logs: AuditLog[] = [];
  loading = false;
  filterAction = '';
  filterStartDate = '';
  filterEndDate = '';

  constructor(
    private http: HttpClient,
    private router: Router,
    private toastService: ToastService
  ) {}

  ngOnInit() {
    this.checkAdmin();
    this.loadLogs();
  }

  checkAdmin() {
    const user = localStorage.getItem('user');
    if (!user) {
      this.router.navigate(['/entrar']);
      return;
    }

    const userData = JSON.parse(user);
    if (userData.role !== 3) {
      this.toastService.error('Acesso negado. Apenas administradores.');
      this.router.navigate(['/dashboard']);
    }
  }

  loadLogs() {
    this.loading = true;
    const params: any = {};
    
    if (this.filterAction) params.action = this.filterAction;
    if (this.filterStartDate) params.startDate = this.filterStartDate;
    if (this.filterEndDate) params.endDate = this.filterEndDate;

    this.http.get<any>('http://localhost:5058/api/auditlogs', { params })
      .subscribe({
        next: (response) => {
          if (response.isSuccess) {
            this.logs = response.data;
          }
          this.loading = false;
        },
        error: (error) => {
          console.error('Erro ao carregar logs:', error);
          this.toastService.error('Erro ao carregar logs de auditoria');
          this.loading = false;
        }
      });
  }

  applyFilters() {
    this.loadLogs();
  }

  clearFilters() {
    this.filterAction = '';
    this.filterStartDate = '';
    this.filterEndDate = '';
    this.loadLogs();
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleString('pt-BR');
  }

  getLoginCount(): number {
    return this.logs.filter(log => log.action.toLowerCase().includes('login')).length;
  }

  getRecentLogsCount(): number {
    const oneDayAgo = new Date();
    oneDayAgo.setDate(oneDayAgo.getDate() - 1);
    return this.logs.filter(log => new Date(log.timestamp) > oneDayAgo).length;
  }

  getActionIcon(action: string): string {
    if (action.includes('Login')) return 'badge-info';
    if (action.includes('Create')) return 'badge-success';
    if (action.includes('Update')) return 'badge-warning';
    if (action.includes('Delete')) return 'badge-danger';
    return 'badge-default';
  }

  getActionBadgeClass(action: string): string {
    if (action.includes('Login')) return 'badge-info';
    if (action.includes('Create')) return 'badge-success';
    if (action.includes('Update')) return 'badge-warning';
    if (action.includes('Delete')) return 'badge-danger';
    return 'badge-default';
  }
}
