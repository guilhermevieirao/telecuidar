import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay, map } from 'rxjs/operators';

export type ReportType = 'users' | 'appointments' | 'specialties' | 'revenue' | 'satisfaction';
export type ExportFormat = 'pdf' | 'excel';

export interface ReportFilter {
  startDate: string;
  endDate: string;
  reportType?: ReportType;
}

export interface ReportStatistics {
  totalUsers: number;
  activeUsers: number;
  totalAppointments: number;
  completedAppointments: number;
  canceledAppointments: number;
  totalRevenue: number;
  averageRating: number;
  totalSpecialties: number;
  activeSpecialties: number;
  newUsersThisMonth: number;
  appointmentsThisMonth: number;
  revenueThisMonth: number;
}

export interface UsersByRoleData {
  role: string;
  count: number;
  percentage: number;
}

export interface AppointmentsByStatusData {
  status: string;
  count: number;
  color: string;
}

export interface AppointmentsByMonthData {
  month: string;
  appointments: number;
  completed: number;
  canceled: number;
}

export interface SpecialtiesRankingData {
  specialty: string;
  appointments: number;
  revenue: number;
}

export interface ReportData {
  statistics: ReportStatistics;
  usersByRole: UsersByRoleData[];
  appointmentsByStatus: AppointmentsByStatusData[];
  appointmentsByMonth: AppointmentsByMonthData[];
  specialtiesRanking: SpecialtiesRankingData[];
}

@Injectable({
  providedIn: 'root'
})
export class ReportsService {
  private mockStatistics: ReportStatistics = {
    totalUsers: 1247,
    activeUsers: 1089,
    totalAppointments: 5842,
    completedAppointments: 5234,
    canceledAppointments: 608,
    totalRevenue: 487650.00,
    averageRating: 4.7,
    totalSpecialties: 8,
    activeSpecialties: 8,
    newUsersThisMonth: 87,
    appointmentsThisMonth: 342,
    revenueThisMonth: 28450.00
  };

  private mockUsersByRole: UsersByRoleData[] = [
    { role: 'Pacientes', count: 892, percentage: 71.5 },
    { role: 'Profissionais', count: 245, percentage: 19.6 },
    { role: 'Administradores', count: 110, percentage: 8.9 }
  ];

  private mockAppointmentsByStatus: AppointmentsByStatusData[] = [
    { status: 'Concluídas', count: 5234, color: '#10b981' },
    { status: 'Agendadas', count: 423, color: '#3b82f6' },
    { status: 'Canceladas', count: 608, color: '#ef4444' },
    { status: 'Em Andamento', count: 45, color: '#f59e0b' }
  ];

  private mockAppointmentsByMonth: AppointmentsByMonthData[] = [
    { month: 'Jan', appointments: 450, completed: 420, canceled: 30 },
    { month: 'Fev', appointments: 520, completed: 485, canceled: 35 },
    { month: 'Mar', appointments: 580, completed: 540, canceled: 40 },
    { month: 'Abr', appointments: 610, completed: 575, canceled: 35 },
    { month: 'Mai', appointments: 590, completed: 550, canceled: 40 },
    { month: 'Jun', appointments: 640, completed: 600, canceled: 40 },
    { month: 'Jul', appointments: 670, completed: 630, canceled: 40 },
    { month: 'Ago', appointments: 650, completed: 610, canceled: 40 },
    { month: 'Set', appointments: 680, completed: 640, canceled: 40 },
    { month: 'Out', appointments: 700, completed: 660, canceled: 40 },
    { month: 'Nov', appointments: 720, completed: 680, canceled: 40 },
    { month: 'Dez', appointments: 342, completed: 324, canceled: 18 }
  ];

  private mockSpecialtiesRanking: SpecialtiesRankingData[] = [
    { specialty: 'Cardiologia', appointments: 1245, revenue: 124500.00 },
    { specialty: 'Dermatologia', appointments: 892, revenue: 89200.00 },
    { specialty: 'Pediatria', appointments: 756, revenue: 75600.00 },
    { specialty: 'Ortopedia', appointments: 645, revenue: 64500.00 },
    { specialty: 'Ginecologia', appointments: 534, revenue: 53400.00 },
    { specialty: 'Psiquiatria', appointments: 478, revenue: 47800.00 },
    { specialty: 'Oftalmologia', appointments: 412, revenue: 41200.00 },
    { specialty: 'Neurologia', appointments: 380, revenue: 38000.00 }
  ];

  getReportData(filter: ReportFilter): Observable<ReportData> {
    return of({
      statistics: this.mockStatistics,
      usersByRole: this.mockUsersByRole,
      appointmentsByStatus: this.mockAppointmentsByStatus,
      appointmentsByMonth: this.mockAppointmentsByMonth,
      specialtiesRanking: this.mockSpecialtiesRanking
    }).pipe(delay(500));
  }

  exportReport(filter: ReportFilter, format: ExportFormat): Observable<Blob> {
    const mimeType = format === 'pdf' 
      ? 'application/pdf' 
      : 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet';
    
    return of(new Blob([`${format.toUpperCase()} content`], { type: mimeType }))
      .pipe(delay(1000));
  }
}
