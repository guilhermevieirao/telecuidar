import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';

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

@Injectable({
  providedIn: 'root'
})
export class StatsService {
  // TODO: Substituir por chamadas reais ao backend
  getPlatformStats(): Observable<PlatformStats> {
    // Dados mockados para demonstração
    const mockStats: PlatformStats = {
      totalUsers: 1247,
      usersTrend: { direction: 'up', value: '+12%' },
      appointmentsScheduled: 156,
      appointmentsTrend: { direction: 'up', value: '+8%' },
      occupancyRate: 78,
      occupancyTrend: { direction: 'down', value: '-3%' },
      averageRating: 4.8,
      ratingTrend: { direction: 'up', value: '+0.2' },
      activeProfessionals: 48,
      activePatients: 324,
      todayAppointments: 23,
      averageConsultationTime: 35
    };

    // Simula delay de rede
    return of(mockStats).pipe(delay(500));
  }
}
