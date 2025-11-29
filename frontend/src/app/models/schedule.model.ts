export interface ScheduleDto {
  id: number;
  professionalId: number;
  professionalName: string;
  startDate: Date;
  endDate?: Date;
  isActive: boolean;
  scheduleDays: ScheduleDayDto[];
  createdAt: Date;
}

export interface ScheduleDayDto {
  id: number;
  dayOfWeek: number;
  dayOfWeekName: string;
  startTime: string;
  endTime: string;
  appointmentDuration: number;
  intervalBetweenAppointments: number;
  breakStartTime?: string;
  breakEndTime?: string;
}

export interface CreateScheduleCommand {
  professionalId: number;
  startDate: Date;
  endDate?: Date;
  isActive: boolean;
  scheduleDays: CreateScheduleDayDto[];
}

export interface CreateScheduleDayDto {
  dayOfWeek: number;
  startTime: string;
  endTime: string;
  appointmentDuration: number;
  intervalBetweenAppointments: number;
  breakStartTime?: string;
  breakEndTime?: string;
}

export interface UpdateScheduleCommand {
  id: number;
  startDate: Date;
  endDate?: Date;
  isActive: boolean;
  scheduleDays: CreateScheduleDayDto[];
}
