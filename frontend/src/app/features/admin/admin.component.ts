import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { NgxMaskDirective } from 'ngx-mask';
import { ToastService } from '../../core/services/toast.service';
import { ConfirmModalComponent } from '../../shared/components/confirm-modal/confirm-modal.component';
import { PaginationComponent, PageInfo } from '../../shared/components/pagination/pagination.component';
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration, ChartData, ChartType } from 'chart.js';
import { PagedResult } from '../../core/models/paged-result.model';
import { NotificationsComponent } from '../notifications/notifications.component';
import { ThemeToggleComponent } from '../../shared/components/theme-toggle/theme-toggle.component';
import { MobileMenu, MenuItem } from '../../shared/components/mobile-menu/mobile-menu';
import { FilesComponent } from '../files/files.component';
import { ReportsComponent } from '../reports/reports.component';
import { ProfileComponent } from '../profile/profile.component';
import { SpecialtiesComponent } from '../../pages/specialties/specialties.component';
import { SchedulesComponent } from '../../pages/schedules/schedules.component';
import { environment } from '../../../environments/environment';

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
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    ConfirmModalComponent,
    NgxMaskDirective,
    BaseChartDirective,
    PaginationComponent,
    NotificationsComponent,
    ThemeToggleComponent,
    MobileMenu,
    FilesComponent,
    ReportsComponent,
    ProfileComponent,
    SpecialtiesComponent,
    SchedulesComponent
  ],
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss']
})
export class AdminComponent implements OnInit {
  activeTab: 'dashboard' | 'users' | 'audit-logs' | 'invitations' | 'files' | 'reports' | 'profile' | 'specialties' | 'schedules' = 'dashboard';
  menuItems: MenuItem[] = [];
  adminUser: any = null;
  
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
  
  // Specialties management in users table
  showSpecialtiesModal = false;
  selectedUserForSpecialties: User | null = null;
  allSpecialties: any[] = [];
  userSpecialties: any[] = [];
  availableSpecialties: any[] = [];
  selectedSpecialtyToAdd: number | null = null;
  
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
      this.adminUser = this.currentUser;
      
      // Verificar se é admin
      if (this.currentUser.role !== 3) {
        this.router.navigate(['/painel']);
        return;
      }
    }

    this.setupMenu();
    this.loadStatistics();
    this.loadUsers();
    this.loadAuditLogs();
  }

  setupMenu(): void {
    this.menuItems = [
      { label: 'Dashboard', icon: '📊', action: () => this.setActiveTab('dashboard') },
      { label: 'Usuários', icon: '👥', action: () => this.setActiveTab('users') },
      { label: 'Auditoria', icon: '📋', action: () => this.setActiveTab('audit-logs') },
      { label: 'Convites', icon: '✉️', action: () => this.setActiveTab('invitations') },
      { divider: true },
      { label: 'Especialidades', icon: '🩺', action: () => this.setActiveTab('specialties') },
      { label: 'Agendas', icon: '📅', action: () => this.setActiveTab('schedules') },
      { label: 'Arquivos', icon: '📁', action: () => this.setActiveTab('files') },
      { label: 'Relatórios', icon: '📈', action: () => this.setActiveTab('reports') },
      { label: 'Perfil', icon: '👤', action: () => this.setActiveTab('profile') },
      { divider: true },
      { label: 'Sair', icon: '🚪', action: () => this.logout() }
    ];
  }

  setActiveTab(tab: 'dashboard' | 'users' | 'audit-logs' | 'invitations' | 'files' | 'reports' | 'profile' | 'specialties' | 'schedules'): void {
    this.activeTab = tab;
    
    if (tab === 'invitations') {
      this.loadInvitations();
    } else if (tab === 'audit-logs') {
      this.loadAuditLogs();
    }
  }

  // Dashboard Methods
  loadStatistics(): void {
    this.http.get<any>(`${environment.apiUrl}/admin/statistics`)
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

    this.http.get<any>(`${environment.apiUrl}/users?${params.toString()}`)
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

    this.http.delete(`${environment.apiUrl}/users/${this.userToDelete.id}`)
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

  // Specialty Management Methods
  openSpecialtiesModal(user: User): void {
    this.selectedUserForSpecialties = user;
    this.showSpecialtiesModal = true;
    this.loadAllSpecialties();
    this.loadUserSpecialties(user.id);
  }

  closeSpecialtiesModal(): void {
    this.showSpecialtiesModal = false;
    this.selectedUserForSpecialties = null;
    this.userSpecialties = [];
    this.availableSpecialties = [];
    this.selectedSpecialtyToAdd = null;
  }

  loadAllSpecialties(): void {
    this.http.get<any>(`${environment.apiUrl}/specialties`).subscribe({
      next: (response) => {
        console.log('Response all specialties:', response);
        // A resposta vem diretamente como array
        if (Array.isArray(response)) {
          this.allSpecialties = response;
        } else if (response.isSuccess && response.data) {
          this.allSpecialties = response.data;
        } else if (response.data && Array.isArray(response.data)) {
          this.allSpecialties = response.data;
        } else {
          this.allSpecialties = [];
        }
        console.log('All specialties loaded:', this.allSpecialties);
        this.updateAvailableSpecialties();
      },
      error: (error) => {
        console.error('Erro ao carregar especialidades:', error);
        this.allSpecialties = [];
      }
    });
  }

  loadUserSpecialties(userId: number): void {
    console.log('🔍 Loading specialties for user:', userId);
    this.http.get<any>(`${environment.apiUrl}/specialties/user/${userId}`).subscribe({
      next: (response) => {
        console.log('📦 Raw response:', response);
        console.log('📦 Response type:', typeof response);
        console.log('📦 Is array?', Array.isArray(response));
        console.log('📦 Has isSuccess?', response?.isSuccess);
        console.log('📦 Has data?', response?.data);
        
        // A resposta pode vir com wrapper isSuccess
        if (Array.isArray(response)) {
          this.userSpecialties = response;
          console.log('✅ Set from array:', this.userSpecialties);
        } else if (response.isSuccess && response.data) {
          this.userSpecialties = response.data;
          console.log('✅ Set from isSuccess wrapper:', this.userSpecialties);
        } else if (response.data && Array.isArray(response.data)) {
          this.userSpecialties = response.data;
          console.log('✅ Set from data array:', this.userSpecialties);
        } else {
          this.userSpecialties = [];
          console.log('⚠️ No valid data found, set to empty array');
        }
        console.log('📋 Final userSpecialties:', this.userSpecialties);
        console.log('📋 Length:', this.userSpecialties.length);
        this.updateAvailableSpecialties();
      },
      error: (error) => {
        console.error('❌ Erro ao carregar especialidades do usuário:', error);
        console.error('❌ Error details:', error.error);
        console.error('❌ Status:', error.status);
        this.userSpecialties = [];
        this.updateAvailableSpecialties();
      }
    });
  }

  updateAvailableSpecialties(): void {
    console.log('Updating available specialties...');
    console.log('All specialties:', this.allSpecialties);
    console.log('User specialties:', this.userSpecialties);
    
    const userSpecialtyIds = this.userSpecialties.map(s => s.specialtyId || s.id);
    console.log('User specialty IDs:', userSpecialtyIds);
    
    this.availableSpecialties = this.allSpecialties.filter(
      s => !userSpecialtyIds.includes(s.id)
    );
    console.log('Available specialties:', this.availableSpecialties);
  }

  addSpecialtyToUser(): void {
    if (!this.selectedSpecialtyToAdd || !this.selectedUserForSpecialties) {
      this.toastService.warning('Selecione uma especialidade');
      return;
    }

    this.http.post(
      `${environment.apiUrl}/specialties/${this.selectedSpecialtyToAdd}/professionals/${this.selectedUserForSpecialties.id}`,
      {}
    ).subscribe({
      next: () => {
        this.toastService.success('Especialidade atribuída com sucesso');
        this.loadUserSpecialties(this.selectedUserForSpecialties!.id);
        this.selectedSpecialtyToAdd = null;
      },
      error: (error) => {
        console.error('Erro ao atribuir especialidade:', error);
        this.toastService.error(error.error?.message || 'Erro ao atribuir especialidade');
      }
    });
  }

  removeSpecialtyFromUser(specialtyId: number): void {
    if (!this.selectedUserForSpecialties) return;

    if (!confirm('Tem certeza que deseja remover esta especialidade?')) {
      return;
    }

    this.http.delete(
      `${environment.apiUrl}/specialties/${specialtyId}/professionals/${this.selectedUserForSpecialties.id}`
    ).subscribe({
      next: () => {
        this.toastService.success('Especialidade removida com sucesso');
        this.loadUserSpecialties(this.selectedUserForSpecialties!.id);
      },
      error: (error) => {
        console.error('Erro ao remover especialidade:', error);
        this.toastService.error('Erro ao remover especialidade');
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
    
    this.http.put(`${environment.apiUrl}/users/${userId}`, updateData)
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
    this.http.post(`${environment.apiUrl}/admin/users`, userData)
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

    this.http.get<any>(`${environment.apiUrl}/admin/audit-logs?${params.toString()}`)
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
    this.http.get<any>(`${environment.apiUrl}/admin/invitations`)
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

    this.http.post(`${environment.apiUrl}/admin/invitations`, invitationData)
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
    this.http.post(`${environment.apiUrl}/admin/invitations`, invitationData)
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
