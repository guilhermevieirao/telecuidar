import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { 
  SpecialtyDto, 
  CreateSpecialtyCommand, 
  UpdateSpecialtyCommand,
  SpecialtyFieldDto,
  CreateSpecialtyFieldDto,
  UpdateSpecialtyFieldDto
} from '../models/specialty.model';

interface UserDto {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  fullName: string;
}

@Injectable({
  providedIn: 'root'
})
export class SpecialtyService {
  private apiUrl = `${environment.apiUrl}/specialties`;

  constructor(private http: HttpClient) {}

  getAll(includeInactive: boolean = false): Observable<SpecialtyDto[]> {
    const params = new HttpParams().set('includeInactive', includeInactive.toString());
    return this.http.get<SpecialtyDto[]>(this.apiUrl, { params });
  }

  getProfessionals(specialtyId: number): Observable<UserDto[]> {
    return this.http.get<UserDto[]>(`${this.apiUrl}/${specialtyId}/professionals`);
  }

  create(command: CreateSpecialtyCommand): Observable<number> {
    return this.http.post<number>(this.apiUrl, command);
  }

  update(command: UpdateSpecialtyCommand): Observable<boolean> {
    return this.http.put<boolean>(`${this.apiUrl}/${command.id}`, command);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  assignToProfessional(specialtyId: number, userId: number): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${specialtyId}/professionals/${userId}`, {});
  }

  removeFromProfessional(specialtyId: number, userId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${specialtyId}/professionals/${userId}`);
  }

  // Métodos para campos personalizados
  getFields(specialtyId: number): Observable<SpecialtyFieldDto[]> {
    return this.http.get<SpecialtyFieldDto[]>(`${this.apiUrl}/${specialtyId}/fields`);
  }

  createField(specialtyId: number, field: CreateSpecialtyFieldDto): Observable<SpecialtyFieldDto> {
    return this.http.post<SpecialtyFieldDto>(`${this.apiUrl}/${specialtyId}/fields`, field);
  }

  updateField(fieldId: number, field: UpdateSpecialtyFieldDto): Observable<SpecialtyFieldDto> {
    return this.http.put<SpecialtyFieldDto>(`${this.apiUrl}/fields/${fieldId}`, field);
  }

  deleteField(fieldId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/fields/${fieldId}`);
  }
}
