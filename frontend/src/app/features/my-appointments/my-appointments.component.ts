import { Component, OnInit } from '@angular/core';
import { CommonModule, NgIf, NgFor } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { BreadcrumbComponent } from '../../shared/components/molecules/breadcrumb/breadcrumb.component';
import { NotificationsComponent } from '../notifications/notifications.component';
import { MobileMenu, MenuItem } from '../../shared/components/organisms/mobile-menu/mobile-menu';
import { ButtonComponent } from '../../shared/components/atoms/button/button.component';
import { BadgeComponent } from '../../shared/components/atoms/badge/badge.component';
import { AppointmentService, Appointment } from '../../core/services/appointment.service';
import { ModalService } from '../../core/services/modal.service';

type FilterStatus = 'all' | 'upcoming' | 'past' | 'cancelled';

@Component({
  selector: 'app-my-appointments',
  standalone: true,
  imports: [CommonModule, NgIf, NgFor, RouterLink, FormsModule, BreadcrumbComponent, NotificationsComponent, MobileMenu, ButtonComponent],
  templateUrl: './my-appointments.component.html',
  styleUrls: ['./my-appointments.component.scss']
})
export class MyAppointmentsComponent implements OnInit {
  user: any = null;
  menuItems: MenuItem[] = [];
  
  appointments: Appointment[] = [];
  filteredAppointments: Appointment[] = [];
  currentFilter: FilterStatus = 'all';
  
  loading = false;
  error: string | null = null;

  // Modal states
  showDetailsModal = false;
  showCancelModal = false;
  selectedAppointment: Appointment | null = null;
  cancellationReason = '';
  cancelLoading = false;

  constructor(
    private appointmentService: AppointmentService,
    private router: Router,
    private modalService: ModalService
  ) {}

  ngOnInit(): void {
    const userStr = localStorage.getItem('user');
    if (userStr) {
      this.user = JSON.parse(userStr);
    }
    this.setupMenu();
    this.loadAppointments();
  }

  setupMenu(): void {
    this.menuItems = [
      { label: 'Dashboard', icon: '🏠', route: '/painel' },
      { label: 'Agendar Consulta', icon: '🗓️', route: '/agendar' },
      { label: 'Minhas Consultas', icon: '📋', route: '/minhas-consultas' },
      { label: 'Meu Perfil', icon: '👤', route: '/perfil' },
      { divider: true },
      { label: 'Sair', icon: '🚪', action: () => this.logout() }
    ];
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.router.navigate(['/entrar']);
  }

  getUserInitials(): string {
    if (!this.user) return 'U';
    const firstInitial = this.user.firstName?.charAt(0) || '';
    const lastInitial = this.user.lastName?.charAt(0) || '';
    return (firstInitial + lastInitial).toUpperCase() || 'U';
  }

  getUserRoleName(): string {
    if (!this.user) return '';
    switch (this.user.role) {
      case 1: return 'Paciente';
      case 2: return 'Profissional';
      case 3: return 'Administrador';
      default: return '';
    }
  }

  loadAppointments(): void {
    this.loading = true;
    this.error = null;

    console.log('Carregando consultas...');

    this.appointmentService.getMyAppointments(true).subscribe({
      next: (appointments) => {
        console.log('Consultas recebidas:', appointments);
        this.appointments = appointments;
        this.applyFilter(this.currentFilter);
        this.loading = false;
      },
      error: (err) => {
        console.error('Erro completo:', err);
        if (err.error && err.error.message) {
          this.error = `Erro: ${err.error.message}`;
        } else if (err.error && typeof err.error === 'string') {
          this.error = `Erro: ${err.error}`;
        } else {
          this.error = 'Erro ao carregar consultas. Tente novamente.';
        }
        this.loading = false;
      }
    });
  }

  setFilter(filter: FilterStatus): void {
    this.currentFilter = filter;
    this.applyFilter(filter);
  }

  applyFilter(filter: FilterStatus): void {
    const now = new Date();
    console.log('Aplicando filtro:', filter);
    console.log('Data atual:', now);
    console.log('Total de consultas:', this.appointments.length);
    
    switch (filter) {
      case 'upcoming':
        this.filteredAppointments = this.appointments.filter(a => {
          const appointmentDate = new Date(a.appointmentDate);
          console.log('Upcoming - Data da consulta:', appointmentDate, 'Status:', a.status);
          return appointmentDate >= now && (a.status === 'Agendado' || a.status === 'Confirmado');
        });
        break;
      
      case 'past':
        this.filteredAppointments = this.appointments.filter(a => {
          const appointmentDate = new Date(a.appointmentDate);
          return appointmentDate < now || a.status === 'Concluído';
        });
        break;
      
      case 'cancelled':
        this.filteredAppointments = this.appointments.filter(a => a.status === 'Cancelado');
        break;
      
      case 'all':
      default:
        this.filteredAppointments = [...this.appointments];
        break;
    }
    
    console.log('Consultas filtradas:', this.filteredAppointments.length);
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'Agendado':
        return 'status-scheduled';
      case 'Confirmado':
        return 'status-confirmed';
      case 'Cancelado':
        return 'status-cancelled';
      case 'Concluído':
        return 'status-completed';
      default:
        return '';
    }
  }

  getStatusIcon(status: string): string {
    switch (status) {
      case 'Agendado':
        return '📅';
      case 'Confirmado':
        return '✅';
      case 'Cancelado':
        return '❌';
      case 'Concluído':
        return '✔️';
      default:
        return '📋';
    }
  }

  formatDate(dateStr: string): string {
    const date = new Date(dateStr);
    return date.toLocaleDateString('pt-BR', { 
      day: '2-digit', 
      month: 'long', 
      year: 'numeric' 
    });
  }

  formatTime(timeStr: string): string {
    return timeStr.substring(0, 5);
  }

  getCountByStatus(status: FilterStatus): number {
    if (status === 'all') return this.appointments.length;
    
    const now = new Date();
    
    switch (status) {
      case 'upcoming':
        return this.appointments.filter(a => {
          const appointmentDate = new Date(a.appointmentDate);
          return appointmentDate >= now && (a.status === 'Agendado' || a.status === 'Confirmado');
        }).length;
      
      case 'past':
        return this.appointments.filter(a => {
          const appointmentDate = new Date(a.appointmentDate);
          return appointmentDate < now || a.status === 'Concluído';
        }).length;
      
      case 'cancelled':
        return this.appointments.filter(a => a.status === 'Cancelado').length;
      
      default:
        return 0;
    }
  }

  isUpcoming(appointment: Appointment): boolean {
    const appointmentDate = new Date(appointment.appointmentDate);
    const now = new Date();
    return appointmentDate >= now && (appointment.status === 'Agendado' || appointment.status === 'Confirmado');
  }

  viewDetails(appointment: Appointment): void {
    this.selectedAppointment = appointment;
    this.showDetailsModal = true;
  }

  closeDetailsModal(): void {
    this.showDetailsModal = false;
    this.selectedAppointment = null;
  }

  openCancelModal(appointment: Appointment): void {
    this.selectedAppointment = appointment;
    this.cancellationReason = '';
    this.showCancelModal = true;
  }

  closeCancelModal(): void {
    this.showCancelModal = false;
    this.selectedAppointment = null;
    this.cancellationReason = '';
  }

  cancelAppointment(): void {
    if (!this.selectedAppointment || !this.cancellationReason.trim()) {
      this.modalService.showAlert({
        title: 'Validação',
        message: 'Por favor, informe o motivo do cancelamento.',
        type: 'warning'
      });
      return;
    }

    this.cancelLoading = true;

    this.appointmentService.cancelAppointment(this.selectedAppointment.id, this.cancellationReason).subscribe({
      next: () => {
        console.log('Consulta cancelada com sucesso');
        this.modalService.showSuccess('Consulta cancelada com sucesso');
        this.cancelLoading = false;
        this.closeCancelModal();
        
        // Recarregar consultas
        this.loadAppointments();
      },
      error: (err) => {
        console.error('Erro ao cancelar consulta:', err);
        this.cancelLoading = false;
        this.modalService.showError(err.error?.message || 'Erro ao cancelar consulta. Tente novamente.');
      }
    });
  }
}
