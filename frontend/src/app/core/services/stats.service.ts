import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '@env/environment';

const API_BASE_URL = environment.apiUrl;

export interface Trend {
  direction: 'up' | 'down';
  value: string;
}

export interface PlatformStats {
  totalUsers: number;
  usersTrend?: Trend;
  appointmentsScheduled: number;
  appointmentsTrend?: Trend;
  occupancyRate: number;
  occupancyTrend?: Trend;
  averageRating: number;
  ratingTrend?: Trend;
  activeProfessionals: number;
  activePatients: number;
  todayAppointments: number;
  averageConsultationTime: number;
}

export interface ChartData {
  labels: string[];
  datasets: {
    label: string;
    data: number[];
    backgroundColor?: string[];
    borderColor?: string[];
    borderWidth?: number;
  }[];
}

export interface DashboardStatsDto {
  // Backend DTO (em português)
  totalUsuarios?: number;
  totalPacientes?: number;
  totalProfissionais?: number;
  totalConsultas?: number;
  consultasRealizadas?: number;
  consultasCanceladas?: number;
  consultasAgendadas?: number;
  
  // Formato esperado pelo frontend (inglês) - opcional para backwards compatibility
  users?: {
    totalUsers: number;
    activeUsers: number;
    patients: number;
    professionals: number;
    admins: number;
  };
  appointments?: {
    total: number;
    scheduled: number;
    confirmed: number;
    inProgress: number;
    completed: number;
    cancelled: number;
  };
}

@Injectable({
  providedIn: 'root'
})
export class StatsService {
  private apiUrl = `${API_BASE_URL}/reports`;

  constructor(private http: HttpClient) {}

  getPlatformStats(): Observable<PlatformStats> {
    console.log('[StatsService] getPlatformStats chamado');
    return this.http.get<DashboardStatsDto>(`${this.apiUrl}/dashboard`).pipe(
      map(data => {
        console.log('[StatsService] Response recebida:', data);
        // Mapear do backend (português) para frontend (inglês)
        const totalUsers = data.users?.totalUsers || data.totalUsuarios || 0;
        const patients = data.users?.patients || data.totalPacientes || 0;
        const professionals = data.users?.professionals || data.totalProfissionais || 0;
        const scheduled = data.appointments?.scheduled || data.consultasAgendadas || 0;
        const inProgress = data.appointments?.inProgress || 0;
        
        const stats: PlatformStats = {
          totalUsers,
          appointmentsScheduled: scheduled,
          occupancyRate: 0,
          averageRating: 4.5,
          activeProfessionals: professionals,
          activePatients: patients,
          todayAppointments: inProgress,
          averageConsultationTime: 35
        };
        return stats;
      })
    );
  }

  getAppointmentsByStatus(): Observable<ChartData> {
    return this.http.get<DashboardStatsDto>(`${this.apiUrl}/dashboard`).pipe(
      map(data => {
        // Mapear do backend (português) para frontend
        const scheduled = data.appointments?.scheduled || data.consultasAgendadas || 0;
        const confirmed = data.appointments?.confirmed || 0;
        const inProgress = data.appointments?.inProgress || 0;
        const completed = data.appointments?.completed || data.consultasRealizadas || 0;
        const cancelled = data.appointments?.cancelled || data.consultasCanceladas || 0;
        
        return {
          labels: ['Agendadas', 'Confirmadas', 'Em Andamento', 'Concluídas', 'Canceladas'],
          datasets: [{
            label: 'Status de Agendamentos',
            data: [scheduled, confirmed, inProgress, completed, cancelled],
            backgroundColor: [
              '#3b82f6', // blue-500
              '#8b5cf6', // violet-500
              '#f59e0b', // amber-500
              '#10b981', // green-500
              '#ef4444'  // red-500
            ],
            borderWidth: 1
          }]
        };
      })
    );
  }

  getUsersByRole(): Observable<ChartData> {
    return this.http.get<DashboardStatsDto>(`${this.apiUrl}/dashboard`).pipe(
      map(data => {
        // Mapear do backend (português) para frontend
        const patients = data.users?.patients || data.totalPacientes || 0;
        const professionals = data.users?.professionals || data.totalProfissionais || 0;
        const admins = data.users?.admins || 0;
        
        return {
          labels: ['Pacientes', 'Profissionais', 'Administradores'],
          datasets: [{
            label: 'Usuários por Perfil',
            data: [patients, professionals, admins],
            backgroundColor: [
              '#8b5cf6', // violet-500
              '#06b6d4', // cyan-500
              '#64748b'  // slate-500
            ],
            borderWidth: 1
          }]
        };
      })
    );
  }

  getMonthlyAppointments(): Observable<ChartData> {
    return this.http.get(`${this.apiUrl}/dashboard`).pipe(
      map((data: any) => {
        // Se temos dados de mês a mês
        const monthlyData = data.appointmentsByMonth || [];
        const months = monthlyData.map((m: any) => m.month);
        const appointments = monthlyData.map((m: any) => m.appointments);
        
        return {
          labels: months.length > 0 ? months : ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun'],
          datasets: [{
            label: 'Consultas Realizadas',
            data: appointments.length > 0 ? appointments : [0, 0, 0, 0, 0, 0],
            borderColor: ['#3b82f6'],
            backgroundColor: ['rgba(59, 130, 246, 0.2)'],
            borderWidth: 2
          }]
        };
      })
    );
  }
}
