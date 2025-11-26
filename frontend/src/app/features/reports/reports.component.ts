import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router, RouterLink } from '@angular/router';
import { environment } from '../../../environments/environment';
import { ToastService } from '../../core/services/toast.service';
import { BreadcrumbComponent } from '../../shared/components/breadcrumb/breadcrumb.component';
import { NotificationsComponent } from '../notifications/notifications.component';

interface ReportSummary {
  totalUsers?: number;
  activeUsers?: number;
  inactiveUsers?: number;
  totalLogs?: number;
  totalFiles?: number;
  totalSizeFormatted?: string;
  totalNotifications?: number;
  readPercentage?: number;
}

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, BreadcrumbComponent, NotificationsComponent],
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.scss']
})
export class ReportsComponent implements OnInit {
  private apiUrl = `${environment.apiUrl}/reports`;

  activeReport = signal<string>('users');
  isLoading = signal<boolean>(false);
  reportData = signal<ReportSummary>({});
  user: any = null;
  
  startDate = '';
  endDate = '';

  reportTypes = [
    { id: 'users', name: 'Usuários', icon: '👥', color: 'blue' },
    { id: 'audit-logs', name: 'Auditoria', icon: '📋', color: 'orange' },
    { id: 'files', name: 'Arquivos', icon: '📁', color: 'purple' },
    { id: 'notifications', name: 'Notificações', icon: '🔔', color: 'green' }
  ];

  constructor(
    private http: HttpClient,
    private toastService: ToastService,
    private router: Router
  ) {
    // Definir período padrão (último mês)
    const today = new Date();
    const lastMonth = new Date();
    lastMonth.setMonth(lastMonth.getMonth() - 1);
    
    this.endDate = this.formatDate(today);
    this.startDate = this.formatDate(lastMonth);
  }

  ngOnInit() {
    const token = localStorage.getItem('token');
    const userStr = localStorage.getItem('user');

    if (!token || !userStr) {
      this.router.navigate(['/entrar']);
      return;
    }

    this.user = JSON.parse(userStr);
    
    // Apenas admins podem acessar
    if (this.user?.role !== 3) {
      this.router.navigate(['/painel']);
      return;
    }

    this.loadReport();
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.router.navigate(['/entrar']);
  }

  getUserInitials(): string {
    if (!this.user?.firstName || !this.user?.lastName) return '??';
    return `${this.user.firstName.charAt(0)}${this.user.lastName.charAt(0)}`.toUpperCase();
  }

  setActiveReport(reportId: string) {
    this.activeReport.set(reportId);
    this.loadReport();
  }

  loadReport() {
    this.isLoading.set(true);
    const params: any = {};
    
    if (this.startDate) params.startDate = this.startDate;
    if (this.endDate) params.endDate = this.endDate;

    this.http.get<any>(`${this.apiUrl}/${this.activeReport()}`, { params }).subscribe({
      next: (data) => {
        this.reportData.set(data);
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Erro ao carregar relatório:', error);
        this.toastService.error('Erro ao carregar relatório');
        this.isLoading.set(false);
      }
    });
  }

  downloadPdf() {
    this.isLoading.set(true);
    const params: any = {};
    
    if (this.startDate) params.startDate = this.startDate;
    if (this.endDate) params.endDate = this.endDate;

    this.http.get(`${this.apiUrl}/${this.activeReport()}/pdf`, {
      params,
      responseType: 'blob'
    }).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `relatorio-${this.activeReport()}-${Date.now()}.pdf`;
        link.click();
        window.URL.revokeObjectURL(url);
        this.toastService.success('PDF baixado com sucesso!');
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Erro ao baixar PDF:', error);
        this.toastService.error('Erro ao baixar PDF');
        this.isLoading.set(false);
      }
    });
  }

  downloadExcel() {
    this.isLoading.set(true);
    const params: any = {};
    
    if (this.startDate) params.startDate = this.startDate;
    if (this.endDate) params.endDate = this.endDate;

    this.http.get(`${this.apiUrl}/${this.activeReport()}/excel`, {
      params,
      responseType: 'blob'
    }).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `relatorio-${this.activeReport()}-${Date.now()}.xlsx`;
        link.click();
        window.URL.revokeObjectURL(url);
        this.toastService.success('Excel baixado com sucesso!');
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Erro ao baixar Excel:', error);
        this.toastService.error('Erro ao baixar Excel');
        this.isLoading.set(false);
      }
    });
  }

  private formatDate(date: Date): string {
    return date.toISOString().split('T')[0];
  }

  getActiveReportInfo() {
    return this.reportTypes.find(r => r.id === this.activeReport());
  }
}
