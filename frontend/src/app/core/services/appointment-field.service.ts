import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AppointmentFieldValueDto, SaveAppointmentFieldValueDto } from '../../shared/models/appointment-field.model';

@Injectable({
  providedIn: 'root'
})
export class AppointmentFieldService {
  private apiUrl = `${environment.apiUrl}/appointments`;

  constructor(private http: HttpClient) {}

  getFieldValues(appointmentId: number): Observable<AppointmentFieldValueDto[]> {
    return this.http.get<AppointmentFieldValueDto[]>(`${this.apiUrl}/${appointmentId}/field-values`);
  }

  saveFieldValues(appointmentId: number, fieldValues: SaveAppointmentFieldValueDto[]): Observable<AppointmentFieldValueDto[]> {
    return this.http.post<AppointmentFieldValueDto[]>(`${this.apiUrl}/${appointmentId}/field-values`, fieldValues);
  }
}
