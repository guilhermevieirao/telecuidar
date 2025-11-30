import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface AvailableSpecialty {
  id: number;
  name: string;
  description?: string;
  icon?: string;
  availableProfessionalsCount: number;
}

export interface AvailableDate {
  date: string;
  dateFormatted: string;
  dayOfWeek: string;
  availableSlotsCount: number;
}

export interface AvailableProfessional {
  id: number;
  name: string;
  profilePhotoUrl?: string;
}

export interface AvailableTimeSlot {
  time: string;
  professionals: AvailableProfessional[];
}

export interface Appointment {
  id: number;
  patientId: number;
  patientName: string;
  professionalId: number | null;
  professionalName: string;
  specialtyId: number;
  specialtyName: string;
  appointmentDate: string;
  appointmentTime: string;
  durationMinutes: number;
  status: string;
  notes?: string;
  meetingRoomId?: string;
  createdAt: string;
}

export interface CreateAppointmentRequest {
  professionalId: number | null;
  specialtyId: number;
  appointmentDate: string;
  appointmentTime: string;
  notes?: string;
}

@Injectable({
  providedIn: 'root'
})
export class AppointmentService {
  private readonly baseUrl = `${environment.apiUrl}/appointments`;

  constructor(private http: HttpClient) {}

  getAvailableSpecialties(): Observable<AvailableSpecialty[]> {
    return this.http.get<AvailableSpecialty[]>(`${this.baseUrl}/available-specialties`);
  }

  getAvailableDates(specialtyId: number, daysAhead: number = 30): Observable<AvailableDate[]> {
    return this.http.get<AvailableDate[]>(
      `${this.baseUrl}/available-dates?specialtyId=${specialtyId}&daysAhead=${daysAhead}`
    );
  }

  getAvailableTimeSlots(specialtyId: number, date: string): Observable<AvailableTimeSlot[]> {
    return this.http.get<AvailableTimeSlot[]>(
      `${this.baseUrl}/available-time-slots?specialtyId=${specialtyId}&date=${date}`
    );
  }

  createAppointment(request: CreateAppointmentRequest): Observable<number> {
    return this.http.post<number>(this.baseUrl, request);
  }

  getMyAppointments(includePast: boolean = false): Observable<Appointment[]> {
    return this.http.get<Appointment[]>(
      `${this.baseUrl}/my-appointments?includePast=${includePast}`
    );
  }

  getMyProfessionalAppointments(includePast: boolean = false): Observable<Appointment[]> {
    return this.http.get<Appointment[]>(
      `${this.baseUrl}/my-professional-appointments?includePast=${includePast}`
    );
  }

  cancelAppointment(appointmentId: number, reason: string): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${appointmentId}/cancel`, { reason });
  }

  cancelAppointmentByProfessional(appointmentId: number, reason: string): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${appointmentId}/cancel-professional`, { reason });
  }
}
