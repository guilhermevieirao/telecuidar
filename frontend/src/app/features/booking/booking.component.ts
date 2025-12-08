import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { BreadcrumbComponent } from '../../shared/components/molecules/breadcrumb/breadcrumb.component';
import { NotificationsComponent } from '../notifications/notifications.component';
import { ThemeToggleComponent } from '../../shared/components/atoms/theme-toggle/theme-toggle.component';
import { MobileMenu, MenuItem } from '../../shared/components/organisms/mobile-menu/mobile-menu';
import {
  AppointmentService,
  AvailableSpecialty,
  AvailableDate,
  AvailableTimeSlot,
  AvailableProfessional,
  CreateAppointmentRequest
} from '../../core/services/appointment.service';

@Component({
  selector: 'app-booking',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, BreadcrumbComponent, NotificationsComponent, ThemeToggleComponent, MobileMenu],
  templateUrl: './booking.component.html',
  styleUrls: ['./booking.component.scss']
})
export class BookingComponent implements OnInit {
  user: any = null;
  menuItems: MenuItem[] = [];
  
  // Calendário
  currentMonth: Date = new Date();
  calendarDays: Array<{date: Date | null, available: boolean, data?: AvailableDate}> = [];
  weekDays = ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sáb'];
  // Etapas do fluxo
  currentStep: 'specialty' | 'date' | 'time' | 'professional' | 'confirm' = 'specialty';
  
  // Dados disponíveis
  availableSpecialties: AvailableSpecialty[] = [];
  availableDates: AvailableDate[] = [];
  availableTimeSlots: AvailableTimeSlot[] = [];
  
  // Seleções do usuário
  selectedSpecialty: AvailableSpecialty | null = null;
  selectedDate: AvailableDate | null = null;
  selectedTimeSlot: AvailableTimeSlot | null = null;
  selectedProfessional: AvailableProfessional | null = null;
  notes: string = '';
  
  // Estado
  loading = false;
  error: string | null = null;
  success = false;

  constructor(
    private appointmentService: AppointmentService,
    private router: Router
  ) {}

  ngOnInit(): void {
    const userStr = localStorage.getItem('user');
    if (userStr) {
      this.user = JSON.parse(userStr);
    }
    this.setupMenu();
    this.loadAvailableSpecialties();
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

  loadAvailableSpecialties(): void {
    this.loading = true;
    this.error = null;
    
    this.appointmentService.getAvailableSpecialties().subscribe({
      next: (specialties) => {
        this.availableSpecialties = specialties;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Erro ao carregar especialidades. Tente novamente.';
        this.loading = false;
        console.error(err);
      }
    });
  }

  selectSpecialty(specialty: AvailableSpecialty): void {
    this.selectedSpecialty = specialty;
    this.currentStep = 'date';
    this.loadAvailableDates();
  }

  loadAvailableDates(): void {
    if (!this.selectedSpecialty) return;
    
    this.loading = true;
    this.error = null;
    
    this.appointmentService.getAvailableDates(this.selectedSpecialty.id, 60).subscribe({
      next: (dates) => {
        this.availableDates = dates;
        this.generateCalendar();
        this.loading = false;
        
        if (dates.length === 0) {
          this.error = 'Não há datas disponíveis para esta especialidade no momento.';
        }
      },
      error: (err) => {
        this.error = 'Erro ao carregar datas disponíveis. Tente novamente.';
        this.loading = false;
        console.error(err);
      }
    });
  }

  generateCalendar(): void {
    const year = this.currentMonth.getFullYear();
    const month = this.currentMonth.getMonth();
    
    const firstDay = new Date(year, month, 1);
    const lastDay = new Date(year, month + 1, 0);
    const startingDayOfWeek = firstDay.getDay();
    
    this.calendarDays = [];
    
    // Adicionar dias vazios no início
    for (let i = 0; i < startingDayOfWeek; i++) {
      this.calendarDays.push({ date: null, available: false });
    }
    
    // Adicionar dias do mês
    for (let day = 1; day <= lastDay.getDate(); day++) {
      const date = new Date(year, month, day);
      const dateStr = date.toISOString().split('T')[0];
      const availableDate = this.availableDates.find(d => d.date.startsWith(dateStr));

      // Disponível apenas se houver slots disponíveis
      const isAvailable = !!availableDate && availableDate.availableSlotsCount > 0;
      this.calendarDays.push({
        date: date,
        available: isAvailable,
        data: availableDate
      });
    }
  }

  previousMonth(): void {
    this.currentMonth = new Date(this.currentMonth.getFullYear(), this.currentMonth.getMonth() - 1, 1);
    this.generateCalendar();
  }

  nextMonth(): void {
    this.currentMonth = new Date(this.currentMonth.getFullYear(), this.currentMonth.getMonth() + 1, 1);
    this.generateCalendar();
  }

  getMonthName(): string {
    return this.currentMonth.toLocaleDateString('pt-BR', { month: 'long', year: 'numeric' });
  }

  selectDateFromCalendar(day: {date: Date | null, available: boolean, data?: AvailableDate}): void {
    // Garantir que só dias disponíveis possam ser selecionados
    if (!day.date || !day.available || !day.data) return;
    // Proteção extra: se não houver slots disponíveis, não seleciona
    if (!day.data.availableSlotsCount || day.data.availableSlotsCount <= 0) return;
    this.selectDate(day.data);
  }

  selectDate(date: AvailableDate): void {
    this.selectedDate = date;
    this.currentStep = 'time';
    this.loadAvailableTimeSlots();
  }

  loadAvailableTimeSlots(): void {
    if (!this.selectedSpecialty || !this.selectedDate) return;
    
    this.loading = true;
    this.error = null;
    
    this.appointmentService.getAvailableTimeSlots(
      this.selectedSpecialty.id,
      this.selectedDate.date
    ).subscribe({
      next: (timeSlots) => {
        this.availableTimeSlots = timeSlots;
        this.loading = false;
        
        if (timeSlots.length === 0) {
          this.error = 'Não há horários disponíveis para esta data.';
        }
      },
      error: (err) => {
        this.error = 'Erro ao carregar horários disponíveis. Tente novamente.';
        this.loading = false;
        console.error(err);
      }
    });
  }

  selectTimeSlot(timeSlot: AvailableTimeSlot): void {
    this.selectedTimeSlot = timeSlot;
    
    // Se houver apenas um profissional, selecionar automaticamente
    if (timeSlot.professionals.length === 1) {
      this.selectedProfessional = timeSlot.professionals[0];
      this.currentStep = 'confirm';
    } else if (timeSlot.professionals.length > 1) {
      // Se houver múltiplos profissionais, mostrar seleção
      this.currentStep = 'professional';
    } else {
      // Se não houver profissionais (não deveria acontecer), ir direto para confirmação
      this.currentStep = 'confirm';
    }
  }

  selectProfessional(professional: AvailableProfessional | null): void {
    this.selectedProfessional = professional;
    this.currentStep = 'confirm';
  }

  confirmAppointment(): void {
    if (!this.selectedSpecialty || !this.selectedDate || !this.selectedTimeSlot) {
      this.error = 'Por favor, complete todas as etapas do agendamento.';
      return;
    }
    
    this.loading = true;
    this.error = null;
    
    const request: CreateAppointmentRequest = {
      professionalId: this.selectedProfessional?.id || null,
      specialtyId: this.selectedSpecialty.id,
      appointmentDate: this.selectedDate.date,
      appointmentTime: this.selectedTimeSlot.time,
      notes: this.notes || undefined
    };
    
    console.log('Enviando agendamento:', request);
    
    this.appointmentService.createAppointment(request).subscribe({
      next: (appointmentId) => {
        console.log('Agendamento criado com sucesso! ID:', appointmentId);
        this.success = true;
        this.loading = false;
        
        // Redirecionar após 2 segundos
        setTimeout(() => {
          this.router.navigate(['/minhas-consultas']);
        }, 2000);
      },
      error: (err) => {
        console.error('Erro ao criar agendamento:', err);
        this.error = err.error?.message || 'Erro ao criar agendamento. Tente novamente.';
        this.loading = false;
      }
    });
  }

  goBack(): void {
    switch (this.currentStep) {
      case 'date':
        this.currentStep = 'specialty';
        this.selectedSpecialty = null;
        break;
      case 'time':
        this.currentStep = 'date';
        this.selectedDate = null;
        break;
      case 'professional':
        this.currentStep = 'time';
        this.selectedTimeSlot = null;
        break;
      case 'confirm':
        if (this.selectedTimeSlot && this.selectedTimeSlot.professionals.length > 1) {
          this.currentStep = 'professional';
          this.selectedProfessional = null;
        } else {
          this.currentStep = 'time';
          this.selectedTimeSlot = null;
          this.selectedProfessional = null;
        }
        break;
    }
  }

  reset(): void {
    this.currentStep = 'specialty';
    this.selectedSpecialty = null;
    this.selectedDate = null;
    this.selectedTimeSlot = null;
    this.selectedProfessional = null;
    this.notes = '';
    this.error = null;
    this.success = false;
    this.loadAvailableSpecialties();
  }
}
