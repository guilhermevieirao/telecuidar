import { Component, OnInit, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { ScheduleService } from '../../services/schedule.service';
import { ScheduleDto, CreateScheduleCommand, CreateScheduleDayDto } from '../../models/schedule.model';
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
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './schedules.component.html',
  styleUrls: ['./schedules.component.scss']
})
export class SchedulesComponent implements OnInit {
  @Input() embeddedMode: boolean = false;

  schedules: ScheduleDto[] = [];
  professionals: Professional[] = [];
  selectedSchedule: ScheduleDto | null = null;
  showCreateModal = false;
  showEditModal = false;
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
    private http: HttpClient
  ) {}

  ngOnInit(): void {
    this.loadSchedules();
    this.loadProfessionals();
    this.setDefaultDates();
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
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar agendas:', error);
        this.loading = false;
      }
    });
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

    // Reset days
    this.daysOfWeek.forEach(day => {
      day.enabled = false;
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
      alert('Selecione um profissional');
      return;
    }

    if (!this.startDate) {
      alert('Informe a data de início');
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
        this.loadSchedules();
        this.closeCreateModal();
        this.loading = false;
        alert('Agenda criada com sucesso!');
      },
      error: (error) => {
        console.error('Erro ao criar agenda:', error);
        alert(error.error?.message || 'Erro ao criar agenda');
        this.loading = false;
      }
    });
  }

  updateSchedule(): void {
    if (!this.selectedSchedule) return;

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
        this.loadSchedules();
        this.closeEditModal();
        this.loading = false;
        alert('Agenda atualizada com sucesso!');
      },
      error: (error) => {
        console.error('Erro ao atualizar agenda:', error);
        alert(error.error?.message || 'Erro ao atualizar agenda');
        this.loading = false;
      }
    });
  }

  deleteSchedule(schedule: ScheduleDto): void {
    if (!confirm(`Tem certeza que deseja excluir a agenda de ${schedule.professionalName}?`)) {
      return;
    }

    this.loading = true;
    this.scheduleService.delete(schedule.id).subscribe({
      next: () => {
        this.loadSchedules();
        this.loading = false;
        alert('Agenda excluída com sucesso!');
      },
      error: (error) => {
        console.error('Erro ao excluir agenda:', error);
        alert('Erro ao excluir agenda');
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
}
