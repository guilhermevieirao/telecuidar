import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ScheduleDto, CreateScheduleCommand, UpdateScheduleCommand } from '../models/schedule.model';

@Injectable({
  providedIn: 'root'
})
export class ScheduleService {
  private apiUrl = `${environment.apiUrl}/schedules`;

  constructor(private http: HttpClient) {}

  getAll(professionalId?: number, isActive?: boolean): Observable<ScheduleDto[]> {
    let params = new HttpParams();
    if (professionalId) {
      params = params.set('professionalId', professionalId.toString());
    }
    if (isActive !== undefined) {
      params = params.set('isActive', isActive.toString());
    }
    return this.http.get<ScheduleDto[]>(this.apiUrl, { params });
  }

  create(command: CreateScheduleCommand): Observable<number> {
    return this.http.post<number>(this.apiUrl, command);
  }

  update(command: UpdateScheduleCommand): Observable<boolean> {
    return this.http.put<boolean>(`${this.apiUrl}/${command.id}`, command);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
