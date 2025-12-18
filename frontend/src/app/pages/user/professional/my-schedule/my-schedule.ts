import { Component, afterNextRender, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { SchedulesService, Schedule, DayOfWeek } from '@app/core/services/schedules.service';

@Component({
  selector: 'app-my-schedule',
  standalone: true,
  imports: [CommonModule, DatePipe],
  templateUrl: './my-schedule.html',
  styleUrl: './my-schedule.scss'
})
export class MyScheduleComponent {
  schedule: Schedule | null = null;
  isLoading = true;

  dayLabels: Record<DayOfWeek, string> = {
    'Monday': 'Segunda-feira',
    'Tuesday': 'Terça-feira',
    'Wednesday': 'Quarta-feira',
    'Thursday': 'Quinta-feira',
    'Friday': 'Sexta-feira',
    'Saturday': 'Sábado',
    'Sunday': 'Sunday'
  };

  private schedulesService = inject(SchedulesService);
  private cdr = inject(ChangeDetectorRef);

  constructor() {
    afterNextRender(() => {
      // Hardcoded ID for demo purposes, matching the mock data in SchedulesService
      const currentProfessionalId = 'prof-1'; 
      
      this.schedulesService.getScheduleByProfessional(currentProfessionalId).subscribe({
        next: (schedules) => {
          this.schedule = Array.isArray(schedules) && schedules.length > 0 ? schedules[0] : null;
          this.isLoading = false;
          this.cdr.detectChanges();
        },
        error: (err) => {
          console.error('Error loading schedule', err);
          this.isLoading = false;
          this.cdr.detectChanges();
        }
      });
    });
  }

  getDayLabel(day: DayOfWeek): string {
    return this.dayLabels[day];
  }
}
