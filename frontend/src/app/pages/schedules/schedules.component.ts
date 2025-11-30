// Interface para paginação de agendas
export interface SchedulesPageInfo {
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

// As propriedades devem ser públicas e declaradas na classe SchedulesComponent

import { Component, OnInit, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { PaginationComponent } from '../../shared/components/pagination/pagination.component';
import { HttpClient } from '@angular/common/http';
import { ScheduleService } from '../../services/schedule.service';
import { ScheduleDto, CreateScheduleCommand, CreateScheduleDayDto, UpdateScheduleCommand } from '../../models/schedule.model';
import { ModalService } from '../../services/modal.service';
import { environment } from '../../../environments/environment';

interface Professional {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  fullName: string;
  role: string;
}

interface DayConfig {
  dayOfWeek: number;
  dayName: string;
  enabled: boolean;
  useCustomSettings: boolean;
  startTime: string;
  endTime: string;
  appointmentDuration: number;
  intervalBetweenAppointments: number;
  hasBreak: boolean;
  breakStartTime: string;
  breakEndTime: string;
}

@Component({
  selector: 'app-schedules',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, PaginationComponent],
  templateUrl: './schedules.component.html',
  styleUrls: ['./schedules.component.scss']
})
export class SchedulesComponent implements OnInit {
  @Input() embeddedMode: boolean = false;

  public schedules: ScheduleDto[] = [];
  public filteredSchedules: ScheduleDto[] = [];
  public professionals: Professional[] = [];
  public selectedSchedule: ScheduleDto | null = null;

  // Propriedades para paginação, busca, filtro e ordenação
  public schedulesPageInfo: SchedulesPageInfo = { pageNumber: 1, pageSize: 10, totalCount: 0, totalPages: 1 };
  public schedulesSortBy = 'startDate';
  public schedulesSortDirection: 'asc' | 'desc' = 'desc';
  public schedulesSearchTerm = '';
  public schedulesProfessionalFilter: number | null = null;
  showCreateModal = false;
  showEditModal = false;
  showDetailsModal = false;
  loading = false;

  // Form data
  selectedProfessionalId: number | null = null;
  startDate: string = '';
  endDate: string = '';
  hasEndDate: boolean = false;
  isActive: boolean = true;

  // Global settings
  globalStartTime: string = '08:00';
  globalEndTime: string = '17:00';
  globalAppointmentDuration: number = 30;
  globalIntervalBetweenAppointments: number = 0;
  globalHasBreak: boolean = false;
  globalBreakStartTime: string = '12:00';
  globalBreakEndTime: string = '13:00';

  daysOfWeek: DayConfig[] = [
    { dayOfWeek: 1, dayName: 'Segunda-feira', enabled: false, useCustomSettings: false, startTime: '08:00', endTime: '17:00', appointmentDuration: 30, intervalBetweenAppointments: 0, hasBreak: false, breakStartTime: '12:00', breakEndTime: '13:00' },
    { dayOfWeek: 2, dayName: 'Terça-feira', enabled: false, useCustomSettings: false, startTime: '08:00', endTime: '17:00', appointmentDuration: 30, intervalBetweenAppointments: 0, hasBreak: false, breakStartTime: '12:00', breakEndTime: '13:00' },
    { dayOfWeek: 3, dayName: 'Quarta-feira', enabled: false, useCustomSettings: false, startTime: '08:00', endTime: '17:00', appointmentDuration: 30, intervalBetweenAppointments: 0, hasBreak: false, breakStartTime: '12:00', breakEndTime: '13:00' },
    { dayOfWeek: 4, dayName: 'Quinta-feira', enabled: false, useCustomSettings: false, startTime: '08:00', endTime: '17:00', appointmentDuration: 30, intervalBetweenAppointments: 0, hasBreak: false, breakStartTime: '12:00', breakEndTime: '13:00' },
    { dayOfWeek: 5, dayName: 'Sexta-feira', enabled: false, useCustomSettings: false, startTime: '08:00', endTime: '17:00', appointmentDuration: 30, intervalBetweenAppointments: 0, hasBreak: false, breakStartTime: '12:00', breakEndTime: '13:00' },
    { dayOfWeek: 6, dayName: 'Sábado', enabled: false, useCustomSettings: false, startTime: '08:00', endTime: '12:00', appointmentDuration: 30, intervalBetweenAppointments: 0, hasBreak: false, breakStartTime: '10:00', breakEndTime: '10:30' },
    { dayOfWeek: 0, dayName: 'Domingo', enabled: false, useCustomSettings: false, startTime: '08:00', endTime: '12:00', appointmentDuration: 30, intervalBetweenAppointments: 0, hasBreak: false, breakStartTime: '10:00', breakEndTime: '10:30' }
  ];

  constructor(
    private scheduleService: ScheduleService,
    private http: HttpClient,
    private modalService: ModalService
  ) {}

  ngOnInit(): void {
    this.loadProfessionals();
    this.setDefaultDates();
    this.loadSchedules();
  }

  setDefaultDates(): void {
    const today = new Date();
    this.startDate = today.toISOString().split('T')[0];
  }

  loadSchedules(): void {
    this.loading = true;
    this.scheduleService.getAll().subscribe({
      next: (data) => {
        this.schedules = data;
        this.applySchedulesFilters();
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar agendas:', error);
        this.loading = false;
      }
    });
  }

  applySchedulesFilters(): void {
    let filtered = [...this.schedules];
    // Filtro por profissional
    if (this.schedulesProfessionalFilter) {
      filtered = filtered.filter(s => s.professionalId === this.schedulesProfessionalFilter);
    }
    // Busca
    if (this.schedulesSearchTerm) {
      const term = this.schedulesSearchTerm.toLowerCase();
      filtered = filtered.filter(s =>
        s.professionalName.toLowerCase().includes(term) ||
        (s.scheduleDays && s.scheduleDays.some(d => d.dayOfWeekName.toLowerCase().includes(term)))
      );
    }
    // Ordenação
    filtered = filtered.sort((a, b) => {
      let aValue = (a as any)[this.schedulesSortBy];
      let bValue = (b as any)[this.schedulesSortBy];
      if (aValue instanceof Date) aValue = aValue.getTime();
      if (bValue instanceof Date) bValue = bValue.getTime();
      if (aValue < bValue) return this.schedulesSortDirection === 'asc' ? -1 : 1;
      if (aValue > bValue) return this.schedulesSortDirection === 'asc' ? 1 : -1;
      return 0;
    });
    // Paginação
    this.schedulesPageInfo.totalCount = filtered.length;
    this.schedulesPageInfo.totalPages = Math.ceil(filtered.length / this.schedulesPageInfo.pageSize) || 1;
    const start = (this.schedulesPageInfo.pageNumber - 1) * this.schedulesPageInfo.pageSize;
    const end = start + this.schedulesPageInfo.pageSize;
    this.filteredSchedules = filtered.slice(start, end);
  }

  onSchedulesSearchChange(event: Event): void {
    this.schedulesSearchTerm = (event.target as HTMLInputElement).value;
    this.schedulesPageInfo.pageNumber = 1;
    this.applySchedulesFilters();
  }

  onSchedulesProfessionalFilterChange(profId: number | null): void {
    this.schedulesProfessionalFilter = profId;
    this.schedulesPageInfo.pageNumber = 1;
    this.applySchedulesFilters();
  }

  sortSchedulesBy(column: string): void {
    if (this.schedulesSortBy === column) {
      this.schedulesSortDirection = this.schedulesSortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.schedulesSortBy = column;
      this.schedulesSortDirection = 'asc';
    }
    this.applySchedulesFilters();
  }

  onSchedulesPageChange(page: number): void {
    this.schedulesPageInfo.pageNumber = page;
    this.applySchedulesFilters();
  }

  getSchedulesSortIcon(column: string): string {
    if (this.schedulesSortBy !== column) return '⇅';
    return this.schedulesSortDirection === 'asc' ? '↑' : '↓';
  }

  loadProfessionals(): void {
    this.http.get<any>(`${environment.apiUrl}/users?role=2&pageSize=100`).subscribe({
      next: (response: any) => {
        if (response.isSuccess && response.data) {
          this.professionals = response.data.items || [];
        }
      },
      error: (error: any) => {
        console.error('Erro ao carregar profissionais:', error);
      }
    });
  }

  openCreateModal(): void {
    this.resetForm();
    this.showCreateModal = true;
  }

  closeCreateModal(): void {
    this.showCreateModal = false;
  }

  openEditModal(schedule: ScheduleDto): void {
    this.selectedSchedule = schedule;
    this.selectedProfessionalId = schedule.professionalId;
    this.startDate = new Date(schedule.startDate).toISOString().split('T')[0];
    this.endDate = schedule.endDate ? new Date(schedule.endDate).toISOString().split('T')[0] : '';
    this.hasEndDate = !!schedule.endDate;
    this.isActive = schedule.isActive;

    // Detectar se há configurações globais (mesmas configurações para todos os dias)
    const firstDay = schedule.scheduleDays[0];
    const hasGlobalSettings = schedule.scheduleDays.every(sd => 
      sd.startTime === firstDay.startTime &&
      sd.endTime === firstDay.endTime &&
      sd.appointmentDuration === firstDay.appointmentDuration &&
      sd.intervalBetweenAppointments === firstDay.intervalBetweenAppointments &&
      sd.breakStartTime === firstDay.breakStartTime &&
      sd.breakEndTime === firstDay.breakEndTime
    );

    // Definir valores globais primeiro (sempre, para ter referência)
    if (firstDay) {
      this.globalStartTime = firstDay.startTime;
      this.globalEndTime = firstDay.endTime;
      this.globalAppointmentDuration = firstDay.appointmentDuration;
      this.globalIntervalBetweenAppointments = firstDay.intervalBetweenAppointments;
      this.globalHasBreak = !!firstDay.breakStartTime;
      this.globalBreakStartTime = firstDay.breakStartTime || '12:00';
      this.globalBreakEndTime = firstDay.breakEndTime || '13:00';
    }

    // Reset days
    this.daysOfWeek.forEach(day => {
      day.enabled = false;
      day.useCustomSettings = false;
      const scheduleDay = schedule.scheduleDays.find(sd => sd.dayOfWeek === day.dayOfWeek);
      if (scheduleDay) {
        day.enabled = true;
        day.startTime = scheduleDay.startTime;
        day.endTime = scheduleDay.endTime;
        day.appointmentDuration = scheduleDay.appointmentDuration;
        day.intervalBetweenAppointments = scheduleDay.intervalBetweenAppointments;
        day.hasBreak = !!scheduleDay.breakStartTime;
        day.breakStartTime = scheduleDay.breakStartTime || '12:00';
        day.breakEndTime = scheduleDay.breakEndTime || '13:00';

        // Marcar useCustomSettings apenas para dias que são diferentes da configuração global
        day.useCustomSettings = 
          scheduleDay.startTime !== this.globalStartTime ||
          scheduleDay.endTime !== this.globalEndTime ||
          scheduleDay.appointmentDuration !== this.globalAppointmentDuration ||
          scheduleDay.intervalBetweenAppointments !== this.globalIntervalBetweenAppointments ||
          scheduleDay.breakStartTime !== (this.globalHasBreak ? this.globalBreakStartTime : null) ||
          scheduleDay.breakEndTime !== (this.globalHasBreak ? this.globalBreakEndTime : null);
      }
    });

    this.showEditModal = true;
  }

  closeEditModal(): void {
    this.showEditModal = false;
    this.selectedSchedule = null;
  }

  resetForm(): void {
    this.selectedProfessionalId = null;
    this.setDefaultDates();
    this.endDate = '';
    this.hasEndDate = false;
    this.isActive = true;
    
    // Reset global settings
    this.globalStartTime = '08:00';
    this.globalEndTime = '17:00';
    this.globalAppointmentDuration = 30;
    this.globalIntervalBetweenAppointments = 0;
    this.globalHasBreak = false;
    this.globalBreakStartTime = '12:00';
    this.globalBreakEndTime = '13:00';
    
    this.daysOfWeek.forEach(day => {
      day.enabled = false;
      day.useCustomSettings = false;
      day.startTime = '08:00';
      day.endTime = day.dayOfWeek >= 6 ? '12:00' : '17:00';
      day.appointmentDuration = 30;
      day.intervalBetweenAppointments = 0;
      day.hasBreak = false;
      day.breakStartTime = '12:00';
      day.breakEndTime = '13:00';
    });
  }

  createSchedule(): void {
    if (!this.selectedProfessionalId) {
      this.modalService.showAlert({
        title: 'Validação',
        message: 'Selecione um profissional',
        type: 'warning'
      });
      return;
    }

    if (!this.startDate) {
      this.modalService.showAlert({
        title: 'Validação',
        message: 'Informe a data de início',
        type: 'warning'
      });
      return;
    }

    const enabledDays = this.daysOfWeek.filter(d => d.enabled);
    if (enabledDays.length === 0) {
      alert('Selecione pelo menos um dia da semana');
      return;
    }

    const scheduleDays: CreateScheduleDayDto[] = enabledDays.map(day => ({
      dayOfWeek: day.dayOfWeek,
      startTime: day.useCustomSettings ? day.startTime : this.globalStartTime,
      endTime: day.useCustomSettings ? day.endTime : this.globalEndTime,
      appointmentDuration: day.useCustomSettings ? day.appointmentDuration : this.globalAppointmentDuration,
      intervalBetweenAppointments: day.useCustomSettings ? day.intervalBetweenAppointments : this.globalIntervalBetweenAppointments,
      breakStartTime: (day.useCustomSettings ? day.hasBreak : this.globalHasBreak) ? 
        (day.useCustomSettings ? day.breakStartTime : this.globalBreakStartTime) : undefined,
      breakEndTime: (day.useCustomSettings ? day.hasBreak : this.globalHasBreak) ? 
        (day.useCustomSettings ? day.breakEndTime : this.globalBreakEndTime) : undefined
    }));

    const command: CreateScheduleCommand = {
      professionalId: this.selectedProfessionalId,
      startDate: new Date(this.startDate),
      endDate: this.hasEndDate && this.endDate ? new Date(this.endDate) : undefined,
      isActive: this.isActive,
      scheduleDays
    };

    this.loading = true;
    this.scheduleService.create(command).subscribe({
      next: () => {
        this.modalService.showSuccess('Agenda criada com sucesso');
        this.loadSchedules();
        this.closeCreateModal();
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao criar agenda:', error);
        this.modalService.showError(error.error?.message || 'Erro ao criar agenda');
        this.loading = false;
      }
    });
  }

  updateSchedule(): void {
    if (!this.selectedSchedule) return;

    const enabledDays = this.daysOfWeek.filter(d => d.enabled);
    if (enabledDays.length === 0) {
      this.modalService.showAlert({
        title: 'Validação',
        message: 'Selecione pelo menos um dia da semana',
        type: 'warning'
      });
      return;
    }

    const scheduleDays: CreateScheduleDayDto[] = enabledDays.map(day => ({
      dayOfWeek: day.dayOfWeek,
      startTime: day.useCustomSettings ? day.startTime : this.globalStartTime,
      endTime: day.useCustomSettings ? day.endTime : this.globalEndTime,
      appointmentDuration: day.useCustomSettings ? day.appointmentDuration : this.globalAppointmentDuration,
      intervalBetweenAppointments: day.useCustomSettings ? day.intervalBetweenAppointments : this.globalIntervalBetweenAppointments,
      breakStartTime: (day.useCustomSettings ? day.hasBreak : this.globalHasBreak) ? 
        (day.useCustomSettings ? day.breakStartTime : this.globalBreakStartTime) : undefined,
      breakEndTime: (day.useCustomSettings ? day.hasBreak : this.globalHasBreak) ? 
        (day.useCustomSettings ? day.breakEndTime : this.globalBreakEndTime) : undefined
    }));

    const command = {
      id: this.selectedSchedule.id,
      startDate: new Date(this.startDate),
      endDate: this.hasEndDate && this.endDate ? new Date(this.endDate) : undefined,
      isActive: this.isActive,
      scheduleDays
    };

    this.loading = true;
    this.scheduleService.update(command).subscribe({
      next: () => {
        this.modalService.showSuccess('Agenda atualizada com sucesso');
        this.loadSchedules();
        this.closeEditModal();
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao atualizar agenda:', error);
        this.modalService.showError(error.error?.message || 'Erro ao atualizar agenda');
        this.loading = false;
      }
    });
  }

  async deleteSchedule(schedule: ScheduleDto): Promise<void> {
    const result = await this.modalService.showConfirm({
      title: 'Confirmar exclusão',
      message: `Tem certeza que deseja excluir a agenda de ${schedule.professionalName}?`,
      confirmText: 'Excluir',
      cancelText: 'Cancelar',
      type: 'danger'
    });
    if (!result) {
      return;
    }

    this.loading = true;
    this.scheduleService.delete(schedule.id).subscribe({
      next: () => {
        this.modalService.showSuccess('Agenda excluída com sucesso');
        this.loadSchedules();
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao excluir agenda:', error);
        this.modalService.showError('Erro ao excluir agenda');
        this.loading = false;
      }
    });
  }

  async toggleScheduleStatus(schedule: ScheduleDto): Promise<void> {
    const action = schedule.isActive ? 'desativar' : 'ativar';
    const result = await this.modalService.showConfirm({
      title: 'Confirmar ação',
      message: `Tem certeza que deseja ${action} a agenda de ${schedule.professionalName}?`,
      confirmText: action === 'desativar' ? 'Desativar' : 'Ativar',
      cancelText: 'Cancelar',
      type: 'primary'
    });
    if (!result) {
      return;
    }

    this.loading = true;
    
    // Criar comando de atualização mantendo todas as configurações, apenas mudando isActive
    const command: UpdateScheduleCommand = {
      id: schedule.id,
      startDate: schedule.startDate,
      endDate: schedule.endDate || undefined,
      isActive: !schedule.isActive,
      scheduleDays: schedule.scheduleDays.map(sd => ({
        dayOfWeek: sd.dayOfWeek,
        startTime: sd.startTime,
        endTime: sd.endTime,
        appointmentDuration: sd.appointmentDuration,
        intervalBetweenAppointments: sd.intervalBetweenAppointments,
        breakStartTime: sd.breakStartTime || undefined,
        breakEndTime: sd.breakEndTime || undefined
      }))
    };

    this.scheduleService.update(command).subscribe({
      next: () => {
        this.modalService.showSuccess(`Agenda ${action}ada com sucesso`);
        this.loadSchedules();
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao atualizar status da agenda:', error);
        this.modalService.showError(error.error?.message || 'Erro ao atualizar status da agenda');
        this.loading = false;
      }
    });
  }

  toggleEndDate(): void {
    if (!this.hasEndDate) {
      this.endDate = '';
    }
  }

  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString('pt-BR');
  }

  formatDateTime(date: Date): string {
    const d = new Date(date);
    return d.toLocaleDateString('pt-BR') + ' ' + d.toLocaleTimeString('pt-BR');
  }

  getStatusBadge(schedule: ScheduleDto): string {
    if (!schedule.isActive) return 'Inativa';
    
    const today = new Date();
    const startDate = new Date(schedule.startDate);
    const endDate = schedule.endDate ? new Date(schedule.endDate) : null;

    if (today < startDate) return 'Pendente';
    if (endDate && today > endDate) return 'Expirada';
    return 'Ativa';
  }

  getStatusColor(schedule: ScheduleDto): string {
    const status = this.getStatusBadge(schedule);
    switch(status) {
      case 'Ativa': return 'bg-green-100 text-green-800';
      case 'Inativa': return 'bg-gray-100 text-gray-800';
      case 'Pendente': return 'bg-yellow-100 text-yellow-800';
      case 'Expirada': return 'bg-red-100 text-red-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  }

  getCompactedScheduleDays(schedule: ScheduleDto): string[] {
    if (!schedule.scheduleDays || schedule.scheduleDays.length === 0) return [];

    const dayNames = ['Domingo', 'Segunda', 'Terça', 'Quarta', 'Quinta', 'Sexta', 'Sábado'];
    
    // Agrupar dias por configuração (horários, duração, intervalo)
    interface GroupKey {
      startTime: string;
      endTime: string;
      appointmentDuration: number;
      intervalBetweenAppointments: number;
      breakStartTime?: string;
      breakEndTime?: string;
    }

    const groups = new Map<string, number[]>();

    schedule.scheduleDays.forEach(day => {
      const key = JSON.stringify({
        startTime: day.startTime,
        endTime: day.endTime,
        appointmentDuration: day.appointmentDuration,
        intervalBetweenAppointments: day.intervalBetweenAppointments,
        breakStartTime: day.breakStartTime,
        breakEndTime: day.breakEndTime
      });

      if (!groups.has(key)) {
        groups.set(key, []);
      }
      groups.get(key)!.push(day.dayOfWeek);
    });

    // Gerar descrições compactadas
    const result: string[] = [];

    groups.forEach((days, keyStr) => {
      const config: GroupKey = JSON.parse(keyStr);
      days.sort((a, b) => a - b);

      // Detectar sequências consecutivas
      const ranges: string[] = [];
      let rangeStart = days[0];
      let rangeEnd = days[0];

      for (let i = 1; i <= days.length; i++) {
        if (i < days.length && days[i] === rangeEnd + 1) {
          rangeEnd = days[i];
        } else {
          // Finalizar range
          if (rangeStart === rangeEnd) {
            ranges.push(dayNames[rangeStart]);
          } else if (rangeEnd === rangeStart + 1) {
            ranges.push(`${dayNames[rangeStart]} e ${dayNames[rangeEnd]}`);
          } else {
            ranges.push(`${dayNames[rangeStart]} a ${dayNames[rangeEnd]}`);
          }

          if (i < days.length) {
            rangeStart = days[i];
            rangeEnd = days[i];
          }
        }
      }

      let description = `${ranges.join(', ')}|${config.startTime} - ${config.endTime}`;
      
      if (config.breakStartTime && config.breakEndTime) {
        description += `|${config.breakStartTime} - ${config.breakEndTime}`;
      }

      result.push(description);
    });

    return result;
  }

  getDayRangeOnly(dayRange: string): string {
    return dayRange.split('|')[0];
  }

  getTimeRange(dayRange: string): string {
    const parts = dayRange.split('|');
    return parts[1] || '';
  }

  getBreakTime(dayRange: string): string {
    const parts = dayRange.split('|');
    return parts[2] || '';
  }

  getProfessionalEmail(schedule: ScheduleDto): string {
    const professional = this.professionals.find(p => p.id === schedule.professionalId);
    return professional?.email || '';
  }

  viewScheduleDetails(schedule: ScheduleDto): void {
    console.log('viewScheduleDetails called', schedule);
    this.selectedSchedule = schedule;
    this.showDetailsModal = true;
    console.log('showDetailsModal:', this.showDetailsModal);
  }

  closeDetailsModal(): void {
    this.showDetailsModal = false;
    this.selectedSchedule = null;
  }

  formatDayRangeForDisplay(dayRange: string): string {
    const parts = dayRange.split('|');
    const days = parts[0];
    const timeRange = parts[1];
    const breakTime = parts[2];
    
    let result = `${days} • Horário: ${timeRange}`;
    if (breakTime) {
      result += ` • Pausa: ${breakTime}`;
    }
    
    return result;
  }

  formatInterval(interval: number | undefined): string {
    if (interval === undefined || interval === null) return 'Sem intervalo';
    return interval === 0 ? 'Sem intervalo' : `${interval}min`;
  }

  hasMultipleConfigurations(schedule: ScheduleDto): boolean {
    if (!schedule.scheduleDays || schedule.scheduleDays.length <= 1) return false;
    
    const first = schedule.scheduleDays[0];
    return schedule.scheduleDays.some(sd => 
      sd.startTime !== first.startTime ||
      sd.endTime !== first.endTime ||
      sd.appointmentDuration !== first.appointmentDuration ||
      sd.intervalBetweenAppointments !== first.intervalBetweenAppointments ||
      sd.breakStartTime !== first.breakStartTime ||
      sd.breakEndTime !== first.breakEndTime
    );
  }

  hasMultipleTimeConfigurations(schedule: ScheduleDto): boolean {
    if (!schedule.scheduleDays || schedule.scheduleDays.length <= 1) return false;
    
    const first = schedule.scheduleDays[0];
    return schedule.scheduleDays.some(sd => 
      sd.startTime !== first.startTime ||
      sd.endTime !== first.endTime ||
      sd.breakStartTime !== first.breakStartTime ||
      sd.breakEndTime !== first.breakEndTime
    );
  }

  hasMultipleDurationConfigurations(schedule: ScheduleDto): boolean {
    if (!schedule.scheduleDays || schedule.scheduleDays.length <= 1) return false;
    
    const first = schedule.scheduleDays[0];
    return schedule.scheduleDays.some(sd => 
      sd.appointmentDuration !== first.appointmentDuration ||
      sd.intervalBetweenAppointments !== first.intervalBetweenAppointments
    );
  }
}
