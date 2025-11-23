import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SpinnerComponent } from '../../shared/components/spinner/spinner.component';
import { ConfirmModalComponent } from '../../shared/components/confirm-modal/confirm-modal.component';
import { EditUserModalComponent } from '../../shared/components/edit-user-modal/edit-user-modal.component';
import { ToastService } from '../../core/services/toast.service';
import { BreadcrumbComponent } from '../../shared/components/breadcrumb/breadcrumb.component';

interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  role: number;
  roleName: string;
  createdAt: string;
  lastLoginAt?: string;
  emailConfirmed: boolean;
}

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, SpinnerComponent, ConfirmModalComponent, EditUserModalComponent, BreadcrumbComponent],
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss']
})
export class AdminComponent implements OnInit {
  users: User[] = [];
  filteredUsers: User[] = [];
  loading = false;
  searchTerm = '';
  selectedRole: number | null = null;
  
  showDeleteModal = false;
  userToDelete: User | null = null;
  
  showEditModal = false;
  userToEdit: User | null = null;

  constructor(
    private http: HttpClient,
    private router: Router,
    private toastService: ToastService
  ) {}

  ngOnInit() {
    this.checkAdmin();
    this.loadUsers();
  }

  checkAdmin() {
    const user = localStorage.getItem('user');
    if (!user) {
      this.router.navigate(['/entrar']);
      return;
    }

    const userData = JSON.parse(user);
    if (userData.role !== 3) { // 3 = Administrador
      this.toastService.error('Acesso negado. Apenas administradores.');
      this.router.navigate(['/dashboard']);
    }
  }

  loadUsers() {
    this.loading = true;
    const params: any = {};
    
    if (this.selectedRole !== null) {
      params.role = this.selectedRole;
    }
    
    if (this.searchTerm) {
      params.searchTerm = this.searchTerm;
    }

    this.http.get<any>('http://localhost:5058/api/users', { params })
      .subscribe({
        next: (response) => {
          if (response.isSuccess) {
            this.users = response.data;
            this.filteredUsers = response.data;
            this.loading = false;
          }
        },
        error: (error) => {
          console.error('Erro ao carregar usuários:', error);
          this.toastService.error('Erro ao carregar usuários');
          this.loading = false;
        }
      });
  }

  filterUsers() {
    this.loadUsers();
  }

  getRoleBadgeClass(role: number): string {
    switch (role) {
      case 1: return 'badge-paciente';
      case 2: return 'badge-profissional';
      case 3: return 'badge-admin';
      default: return '';
    }
  }

  openEditModal(user: User) {
    this.userToEdit = user;
    this.showEditModal = true;
  }

  saveUser(editData: any) {
    if (!this.userToEdit) return;

    this.loading = true;
    const updateData = {
      firstName: editData.firstName,
      lastName: editData.lastName,
      email: editData.email,
      phoneNumber: editData.phoneNumber,
      role: editData.role,
      emailConfirmed: editData.emailConfirmed
    };

    this.http.patch(`http://localhost:5058/api/users/${this.userToEdit.id}`, updateData)
      .subscribe({
        next: (response: any) => {
          if (response.isSuccess) {
            this.toastService.success('Usuário atualizado com sucesso');
            this.loadUsers();
          }
          this.showEditModal = false;
          this.userToEdit = null;
          this.loading = false;
        },
        error: (error) => {
          console.error('Erro ao atualizar usuário:', error);
          this.toastService.error('Erro ao atualizar usuário');
          this.loading = false;
        }
      });
  }

  confirmDelete(user: User) {
    this.userToDelete = user;
    this.showDeleteModal = true;
  }

  deleteUser() {
    if (!this.userToDelete) return;

    this.loading = true;
    this.http.delete(`http://localhost:5058/api/users/${this.userToDelete.id}`)
      .subscribe({
        next: (response: any) => {
          if (response.isSuccess) {
            this.toastService.success('Usuário excluído com sucesso');
            this.loadUsers();
          }
          this.showDeleteModal = false;
          this.userToDelete = null;
          this.loading = false;
        },
        error: (error) => {
          console.error('Erro ao excluir usuário:', error);
          this.toastService.error('Erro ao excluir usuário');
          this.showDeleteModal = false;
          this.loading = false;
        }
      });
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.router.navigate(['/login']);
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('pt-BR');
  }

  getUsersByRole(role: number): User[] {
    return this.users.filter(u => u.role === role);
  }
}
