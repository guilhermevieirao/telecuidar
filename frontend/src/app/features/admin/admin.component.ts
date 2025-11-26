import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { NgxMaskDirective } from 'ngx-mask';
import { ToastService } from '../../core/services/toast.service';
import { ConfirmModalComponent } from '../../shared/components/confirm-modal/confirm-modal.component';
import { PaginationComponent, PageInfo } from '../../shared/components/pagination/pagination.component';
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration, ChartData, ChartType } from 'chart.js';
import { PagedResult } from '../../core/models/paged-result.model';
import { NotificationsComponent } from '../notifications/notifications.component';

interface User {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  fullName: string;
  phoneNumber?: string;
  role: number;
  emailConfirmed: boolean;
  isActive: boolean;
  createdAt: string;
}

interface AuditLog {
  id: number;
  userId?: number;
  userName?: string;
  userEmail?: string;
  action: string;
  entityName: string;
  entityId?: number;
  ipAddress?: string;
  createdAt: string;
}

interface Invitation {
  id: number;
  token: string;
  email: string;
  role: number;
  roleName: string;
  expiresAt: string;
  isUsed: boolean;
  createdByUserName?: string;
  createdAt: string;
  firstName?: string;
  lastName?: string;
  phoneNumber?: string;
}

interface Statistics {
  totalUsers: number;
  totalPacientes: number;
  totalProfissionais: number;
  totalAdministradores: number;
  activeUsers: number;
  inactiveUsers: number;
  usersToday: number;
  usersThisWeek: number;
  usersThisMonth: number;
}

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, ConfirmModalComponent, NgxMaskDirective, BaseChartDirective, PaginationComponent, NotificationsComponent],
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss']
})
export class AdminComponent implements OnInit {
  activeTab: 'dashboard' | 'users' | 'audit-logs' | 'invitations' = 'dashboard';
  
  // Dashboard
  statistics: Statistics | null = null;
  
  // Users
  users: User[] = [];
  filteredUsers: User[] = [];
  selectedRoleFilter: number | null = null;
  searchTerm: string = '';
  userToDelete: User | null = null;
  showDeleteModal = false;
  showEditModal = false;
  userToEdit: User | null = null;
  usersPageInfo: PageInfo | null = null;
  currentUsersPage = 1;
  usersPageSize = 10;
  usersSortBy: string = 'CreatedAt';
  usersSortDirection: 'asc' | 'desc' = 'desc';
  
  // Sorting - mantido por compatibilidade, mas não usado
  sortColumn: 'id' | 'fullName' | 'email' | 'role' | 'isActive' | null = null;
  sortDirection: 'asc' | 'desc' = 'asc';
  
  // Create User Modals
  showCreateUserModal = false;
  showManualCreationModal = false;
  showInvitationCreationModal = false;
  selectedUserType: number | null = null;
  invitationLink: string = '';
  showInvitationLinkModal = false;
  
  // Audit Logs
  auditLogs: AuditLog[] = [];
  auditLogsPageInfo: PageInfo | null = null;
  currentAuditLogsPage = 1;
  auditLogsPageSize = 20;
  auditLogsSortBy: string = 'CreatedAt';
  auditLogsSortDirection: 'asc' | 'desc' = 'desc';
  
  // Invitations
  invitations: Invitation[] = [];
  
  // Forms
  createUserForm: FormGroup;
  editUserForm: FormGroup;
  invitationForm: FormGroup;
  
  loading = false;
  currentUser: any = null;

  // Chart.js Configuration
  public userRolesChartData: ChartData<'doughnut'> = {
    labels: ['Pacientes', 'Profissionais', 'Administradores'],
    datasets: [{
      data: [0, 0, 0],
      backgroundColor: ['#10b981', '#8b5cf6', '#ef4444'],
      hoverBackgroundColor: ['#059669', '#7c3aed', '#dc2626']
    }]
  };

  public userRolesChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: true,
    plugins: {
      legend: {
        position: 'bottom',
        labels: {
          padding: 15,
          font: { size: 12 }
        }
      },
      tooltip: {
        callbacks: {
          label: (context) => {
            const label = context.label || '';
            const value = context.parsed || 0;
            const total = (context.dataset.data as number[]).reduce((a, b) => a + b, 0);
            const percentage = total > 0 ? ((value / total) * 100).toFixed(1) : '0';
            return `${label}: ${value} (${percentage}%)`;
          }
        }
      }
    }
  };

  public userRolesChartType: ChartType = 'doughnut';

  public userActivityChartData: ChartData<'line'> = {
    labels: [],
    datasets: [{
      label: 'Cadastros',
      data: [],
      borderColor: '#3b82f6',
      backgroundColor: 'rgba(59, 130, 246, 0.1)',
      tension: 0.4,
      fill: true
    }]
  };

  public userActivityChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: true,
    plugins: {
      legend: {
        display: true,
        position: 'top'
      },
      tooltip: {
        mode: 'index',
        intersect: false
      }
    },
    scales: {
      y: {
        beginAtZero: true,
        ticks: {
          precision: 0
        }
      }
    }
  };

  public userActivityChartType: ChartType = 'line';

  public activeUsersChartData: ChartData<'bar'> = {
    labels: ['Ativos', 'Inativos'],
    datasets: [{
      label: 'Usuários',
      data: [0, 0],
      backgroundColor: ['#10b981', '#ef4444'],
      borderColor: ['#059669', '#dc2626'],
      borderWidth: 1
    }]
  };

  public activeUsersChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: true,
    plugins: {
      legend: {
        display: false
      },
      tooltip: {
        callbacks: {
          label: (context) => {
            const value = context.parsed.y || 0;
            return `${context.label}: ${value} usuários`;
          }
        }
      }
    },
    scales: {
      y: {
        beginAtZero: true,
        ticks: {
          precision: 0
        }
      }
    }
  };

  public activeUsersChartType: ChartType = 'bar';

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private router: Router,
    private toastService: ToastService
  ) {
    this.createUserForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      phoneNumber: [''],
      role: [1, [Validators.required]]
    });

    this.editUserForm = this.fb.group({
      id: ['', [Validators.required]],
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: [''],
      role: [1, [Validators.required]]
    });

    this.invitationForm = this.fb.group({
      email: ['', [Validators.email]],
      firstName: [''],
      lastName: [''],
      phoneNumber: [''],
      role: [1, [Validators.required]],
      expirationHours: [168, [Validators.required, Validators.min(1)]] // 7 dias padrão
    });
  }

  // Modal Control Methods
  openCreateUserModal(): void {
    this.showCreateUserModal = true;
  }

  selectManualCreation(): void {
    this.showCreateUserModal = false;
    this.showManualCreationModal = true;
    this.selectedUserType = null;
  }

  selectInvitationCreation(): void {
    this.showCreateUserModal = false;
    this.showInvitationCreationModal = true;
    this.selectedUserType = null;
    this.invitationForm.reset({ 
      role: 1, 
      expirationHours: 168 
    });
  }

  selectUserTypeForManual(type: number): void {
    this.selectedUserType = type;
    this.createUserForm.patchValue({ role: type });
  }

  selectUserTypeForInvitation(type: number): void {
    this.selectedUserType = type;
    this.invitationForm.patchValue({ role: type });
  }

  closeAllModals(): void {
    this.showCreateUserModal = false;
    this.showManualCreationModal = false;
    this.showInvitationCreationModal = false;
    this.showInvitationLinkModal = false;
    this.selectedUserType = null;
    this.invitationLink = '';
    this.createUserForm.reset({ role: 1 });
    this.invitationForm.reset({ role: 1, expirationHours: 168 });
  }

  ngOnInit(): void {
    const userStr = localStorage.getItem('user');
    if (userStr) {
      this.currentUser = JSON.parse(userStr);
      
      // Verificar se é admin
      if (this.currentUser.role !== 3) {
        this.router.navigate(['/painel']);
        return;
      }
    }

    this.loadStatistics();
    this.loadUsers();
    this.loadAuditLogs();
  }

  setActiveTab(tab: 'dashboard' | 'users' | 'audit-logs' | 'invitations'): void {
    this.activeTab = tab;
    
    if (tab === 'invitations') {
      this.loadInvitations();
    } else if (tab === 'audit-logs') {
      this.loadAuditLogs();
    }
  }

  // Dashboard Methods
  loadStatistics(): void {
    this.http.get<any>('http://localhost:5058/api/admin/statistics')
      .subscribe({
        next: (response) => {
          if (response.isSuccess) {
            this.statistics = response.data;
            this.updateCharts();
          } else {
            console.error('Erro na resposta de estatísticas:', response);
            this.toastService.error(response.message || 'Erro ao carregar estatísticas');
          }
        },
        error: (error) => {
          console.error('Erro completo ao carregar estatísticas:', error);
          console.error('Status:', error.status);
          console.error('Mensagem:', error.message);
          console.error('Erro do servidor:', error.error);
          
          if (error.status === 401) {
            this.toastService.error('Sessão expirada. Faça login novamente.');
            this.router.navigate(['/entrar']);
          } else {
            this.toastService.error(error.error?.message || 'Erro ao carregar estatísticas');
          }
        }
      });
  }

  // Users Methods
  loadUsers(): void {
    this.loading = true;
    const params = new URLSearchParams({
      pageNumber: this.currentUsersPage.toString(),
      pageSize: this.usersPageSize.toString(),
      sortBy: this.usersSortBy,
      sortDirection: this.usersSortDirection
    });

    if (this.selectedRoleFilter !== null) {
      params.append('role', this.selectedRoleFilter.toString());
    }

    if (this.searchTerm) {
      params.append('searchTerm', this.searchTerm);
    }

    this.http.get<any>(`http://localhost:5058/api/users?${params.toString()}`)
      .subscribe({
        next: (response) => {
          if (response.isSuccess) {
            const pagedResult: PagedResult<User> = response.data;
            this.filteredUsers = pagedResult.items;
            this.usersPageInfo = {
              items: pagedResult.items,
              pageNumber: pagedResult.pageNumber,
              pageSize: pagedResult.pageSize,
              totalCount: pagedResult.totalCount,
              totalPages: pagedResult.totalPages,
              hasPreviousPage: pagedResult.hasPreviousPage,
              hasNextPage: pagedResult.hasNextPage
            };
          }
          this.loading = false;
        },
        error: (error) => {
          console.error('Erro ao carregar usuários:', error);
          this.toastService.error('Erro ao carregar usuários');
          this.loading = false;
        }
      });
  }

  sortUsersBy(column: string): void {
    // Se clicar na mesma coluna, inverte a direção
    if (this.usersSortBy === column) {
      this.usersSortDirection = this.usersSortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      // Nova coluna, começar com ordem descendente (exceto para ID que começa ascendente)
      this.usersSortBy = column;
      this.usersSortDirection = column === 'Id' ? 'asc' : 'desc';
    }
    
    this.currentUsersPage = 1; // Reset para primeira página ao ordenar
    this.loadUsers();
  }

  getUsersSortIcon(column: string): string {
    if (this.usersSortBy !== column) {
      return '⇅'; // Ícone neutro quando não está ordenado
    }
    return this.usersSortDirection === 'asc' ? '↑' : '↓';
  }

  onUsersPageChange(page: number): void {
    this.currentUsersPage = page;
    this.loadUsers();
  }

  onRoleFilterChange(role: number | null): void {
    this.selectedRoleFilter = role;
    this.currentUsersPage = 1; // Reset para primeira página ao filtrar
    this.loadUsers();
  }

  onSearchChange(event: Event): void {
    this.searchTerm = (event.target as HTMLInputElement).value;
    this.currentUsersPage = 1; // Reset para primeira página ao buscar
    this.loadUsers();
  }

  filterUsers(): void {
    // Método mantido para compatibilidade, mas agora a filtragem é feita no backend
    this.loadUsers();
  }

  // Removido sortBy e applySorting - a ordenação agora é feita no backend se necessário

  getRoleName(role: number): string {
    switch (role) {
      case 1: return 'Paciente';
      case 2: return 'Profissional';
      case 3: return 'Administrador';
      default: return 'Desconhecido';
    }
  }

  getRoleBadgeClass(role: number): string {
    switch (role) {
      case 1: return 'badge-paciente';
      case 2: return 'badge-profissional';
      case 3: return 'badge-admin';
      default: return '';
    }
  }

  confirmDeleteUser(user: User): void {
    this.userToDelete = user;
    this.showDeleteModal = true;
  }

  deleteUser(): void {
    if (!this.userToDelete) return;

    this.http.delete(`http://localhost:5058/api/users/${this.userToDelete.id}`)
      .subscribe({
        next: (response: any) => {
          if (response.isSuccess) {
            this.toastService.success('Usuário excluído com sucesso');
            this.loadUsers();
            this.loadStatistics();
          }
          this.showDeleteModal = false;
          this.userToDelete = null;
        },
        error: (error) => {
          console.error('Erro ao excluir usuário:', error);
          this.toastService.error('Erro ao excluir usuário');
          this.showDeleteModal = false;
        }
      });
  }

  openEditModal(user: User): void {
    this.userToEdit = user;
    this.editUserForm.patchValue({
      id: user.id,
      firstName: user.firstName,
      lastName: user.lastName,
      email: user.email,
      phoneNumber: user.phoneNumber || '',
      role: user.role
    });
    this.showEditModal = true;
  }

  updateUser(): void {
    if (this.editUserForm.invalid) {
      this.toastService.warning('Preencha todos os campos obrigatórios');
      return;
    }

    this.loading = true;
    const userId = this.editUserForm.value.id;
    const updateData = {
      ...this.editUserForm.value,
      role: parseInt(this.editUserForm.value.role, 10)
    };
    
    this.http.put(`http://localhost:5058/api/users/${userId}`, updateData)
      .subscribe({
        next: (response: any) => {
          if (response.isSuccess) {
            this.toastService.success('Usuário atualizado com sucesso');
            this.showEditModal = false;
            this.userToEdit = null;
            this.loadUsers();
            this.loadStatistics();
          }
          this.loading = false;
        },
        error: (error) => {
          console.error('Erro ao atualizar usuário:', error);
          this.toastService.error(error.error?.message || 'Erro ao atualizar usuário');
          this.loading = false;
        }
      });
  }

  createUserManually(): void {
    if (!this.selectedUserType) {
      this.toastService.warning('Selecione o tipo de usuário');
      return;
    }

    if (this.createUserForm.invalid) {
      this.toastService.warning('Preencha todos os campos obrigatórios');
      return;
    }

    this.loading = true;
    const userData = {
      ...this.createUserForm.value,
      role: this.selectedUserType
    };
    this.http.post('http://localhost:5058/api/admin/users', userData)
      .subscribe({
        next: (response: any) => {
          if (response.isSuccess) {
            this.toastService.success('Usuário criado com sucesso');
            this.closeAllModals();
            this.loadUsers();
            this.loadStatistics();
          }
          this.loading = false;
        },
        error: (error) => {
          console.error('Erro ao criar usuário:', error);
          this.toastService.error(error.error?.message || 'Erro ao criar usuário');
          this.loading = false;
        }
      });
  }

  // Audit Logs Methods
  loadAuditLogs(): void {
    const params = new URLSearchParams({
      pageNumber: this.currentAuditLogsPage.toString(),
      pageSize: this.auditLogsPageSize.toString(),
      sortBy: this.auditLogsSortBy,
      sortDirection: this.auditLogsSortDirection
    });

    this.http.get<any>(`http://localhost:5058/api/admin/audit-logs?${params.toString()}`)
      .subscribe({
        next: (response) => {
          if (response.isSuccess) {
            const pagedResult: PagedResult<AuditLog> = response.data;
            this.auditLogs = pagedResult.items;
            this.auditLogsPageInfo = {
              items: pagedResult.items,
              pageNumber: pagedResult.pageNumber,
              pageSize: pagedResult.pageSize,
              totalCount: pagedResult.totalCount,
              totalPages: pagedResult.totalPages,
              hasPreviousPage: pagedResult.hasPreviousPage,
              hasNextPage: pagedResult.hasNextPage
            };
          } else {
            console.error('Erro na resposta de logs:', response);
            this.toastService.error(response.message || 'Erro ao carregar logs de auditoria');
          }
        },
        error: (error) => {
          console.error('Erro completo ao carregar logs:', error);
          console.error('Status:', error.status);
          console.error('Mensagem:', error.message);
          console.error('Erro do servidor:', error.error);
          
          if (error.status === 401) {
            this.toastService.error('Sessão expirada. Faça login novamente.');
            this.router.navigate(['/entrar']);
          } else if (error.status === 0) {
            this.toastService.error('Não foi possível conectar ao servidor');
          } else {
            this.toastService.error(error.error?.message || 'Erro ao carregar logs de auditoria');
          }
        }
      });
  }

  sortAuditLogsBy(column: string): void {
    // Se clicar na mesma coluna, inverte a direção
    if (this.auditLogsSortBy === column) {
      this.auditLogsSortDirection = this.auditLogsSortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      // Nova coluna, começar com ordem descendente
      this.auditLogsSortBy = column;
      this.auditLogsSortDirection = 'desc';
    }
    
    this.currentAuditLogsPage = 1; // Reset para primeira página ao ordenar
    this.loadAuditLogs();
  }

  getAuditLogsSortIcon(column: string): string {
    if (this.auditLogsSortBy !== column) {
      return '⇅'; // Ícone neutro quando não está ordenado
    }
    return this.auditLogsSortDirection === 'asc' ? '↑' : '↓';
  }

  onAuditLogsPageChange(page: number): void {
    this.currentAuditLogsPage = page;
    this.loadAuditLogs();
  }

  // Invitations Methods
  loadInvitations(): void {
    this.http.get<any>('http://localhost:5058/api/admin/invitations')
      .subscribe({
        next: (response) => {
          if (response.isSuccess) {
            this.invitations = response.data;
          } else {
            this.toastService.error(response.message || 'Erro ao carregar convites');
          }
        },
        error: (error) => {
          console.error('Erro ao carregar convites:', error);
          this.toastService.error(error.error?.message || 'Erro ao carregar convites');
        }
      });
  }

  isInvitationExpired(expiresAt: string): boolean {
    return new Date(expiresAt) < new Date();
  }

  getInvitationStatusClass(invitation: Invitation): string {
    if (invitation.isUsed) return 'bg-green-100 text-green-800';
    if (this.isInvitationExpired(invitation.expiresAt)) return 'bg-red-100 text-red-800';
    return 'bg-yellow-100 text-yellow-800';
  }

  getInvitationStatusText(invitation: Invitation): string {
    if (invitation.isUsed) return 'Utilizado';
    if (this.isInvitationExpired(invitation.expiresAt)) return 'Expirado';
    return 'Ativo';
  }

  copyInvitationLinkFromTable(token: string): void {
    const link = `${window.location.origin}/cadastrar?token=${token}`;
    navigator.clipboard.writeText(link).then(() => {
      this.toastService.success('Link copiado para a área de transferência');
    }).catch(() => {
      this.toastService.error('Erro ao copiar link');
    });
  }

  // Invitation Modal Methods
  generateInvitationLink(): void {
    if (!this.selectedUserType) {
      this.toastService.warning('Selecione o tipo de usuário');
      return;
    }

    this.loading = true;
    const invitationData = {
      ...this.invitationForm.value,
      email: this.invitationForm.value.email || `temp_${Date.now()}@temp.com`,
      role: this.selectedUserType
    };

    this.http.post('http://localhost:5058/api/admin/invitations', invitationData)
      .subscribe({
        next: (response: any) => {
          if (response.isSuccess) {
            const token = response.data.token;
            this.invitationLink = `${window.location.origin}/cadastrar?token=${token}`;
            this.showInvitationCreationModal = false;
            this.showInvitationLinkModal = true;
            this.toastService.success('Link de convite gerado com sucesso');
          }
          this.loading = false;
        },
        error: (error) => {
          console.error('Erro ao gerar link:', error);
          this.toastService.error(error.error?.message || 'Erro ao gerar link de convite');
          this.loading = false;
        }
      });
  }

  sendInvitationByEmail(): void {
    if (!this.selectedUserType) {
      this.toastService.warning('Selecione o tipo de usuário');
      return;
    }

    if (!this.invitationForm.value.email) {
      this.toastService.warning('Preencha o email para enviar o convite');
      return;
    }

    if (this.invitationForm.get('email')?.invalid) {
      this.toastService.warning('Email inválido');
      return;
    }

    this.loading = true;
    const invitationData = {
      ...this.invitationForm.value,
      role: this.selectedUserType
    };
    this.http.post('http://localhost:5058/api/admin/invitations', invitationData)
      .subscribe({
        next: (response: any) => {
          if (response.isSuccess) {
            this.toastService.success('Convite enviado por email com sucesso');
            this.closeAllModals();
          }
          this.loading = false;
        },
        error: (error) => {
          console.error('Erro ao enviar convite:', error);
          this.toastService.error(error.error?.message || 'Erro ao enviar convite');
          this.loading = false;
        }
      });
  }

  copyInvitationLink(): void {
    navigator.clipboard.writeText(this.invitationLink).then(() => {
      this.toastService.success('Link copiado para a área de transferência');
    }).catch(() => {
      this.toastService.error('Erro ao copiar link');
    });
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.router.navigate(['/entrar']);
  }

  updateCharts(): void {
    if (!this.statistics) return;

    // Atualizar gráfico de distribuição de perfis
    this.userRolesChartData = {
      labels: ['Pacientes', 'Profissionais', 'Administradores'],
      datasets: [{
        data: [
          this.statistics.totalPacientes,
          this.statistics.totalProfissionais,
          this.statistics.totalAdministradores
        ],
        backgroundColor: ['#10b981', '#8b5cf6', '#ef4444'],
        hoverBackgroundColor: ['#059669', '#7c3aed', '#dc2626']
      }]
    };

    // Atualizar gráfico de usuários ativos/inativos
    this.activeUsersChartData = {
      labels: ['Ativos', 'Inativos'],
      datasets: [{
        label: 'Usuários',
        data: [this.statistics.activeUsers, this.statistics.inactiveUsers],
        backgroundColor: ['#10b981', '#ef4444'],
        borderColor: ['#059669', '#dc2626'],
        borderWidth: 1
      }]
    };

    // Atualizar gráfico de atividade (últimos 7 dias)
    const last7Days = this.getLast7Days();
    this.userActivityChartData = {
      labels: last7Days.map(d => d.label),
      datasets: [{
        label: 'Cadastros',
        data: last7Days.map(d => Math.floor(Math.random() * 5)), // Simular dados - substituir com dados reais
        borderColor: '#3b82f6',
        backgroundColor: 'rgba(59, 130, 246, 0.1)',
        tension: 0.4,
        fill: true
      }]
    };
  }

  getLast7Days(): { label: string; date: Date }[] {
    const days = [];
    for (let i = 6; i >= 0; i--) {
      const date = new Date();
      date.setDate(date.getDate() - i);
      days.push({
        label: date.toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit' }),
        date: date
      });
    }
    return days;
  }
}
