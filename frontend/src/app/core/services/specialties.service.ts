import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

const API_BASE_URL = 'http://localhost:5239/api';

export type SpecialtyStatus = 'Active' | 'Inactive';

export interface CustomField {
  name: string;
  type: 'text' | 'number' | 'checkbox' | 'radio' | 'date' | 'select' | 'textarea';
  options?: string[];
  required?: boolean;
  description?: string;
  defaultValue?: any;
  order?: number;
}

export interface Specialty {
  id: string;
  name: string;
  description: string;
  status: SpecialtyStatus;
  createdAt: string;
  updatedAt?: string;
  customFields?: CustomField[];
}

export interface CreateSpecialtyDto {
  name: string;
  description: string;
  status: SpecialtyStatus;
  customFields?: CustomField[];
}

export interface UpdateSpecialtyDto {
  name?: string;
  description?: string;
  status?: SpecialtyStatus;
  customFields?: CustomField[];
}

export interface SpecialtiesFilter {
  search?: string;
  status?: string;
}

export interface PaginatedResponse<T> {
  data: T[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface SpecialtiesSortOptions {
  field: keyof Specialty;
  direction: 'asc' | 'desc';
}

@Injectable({
  providedIn: 'root'
})
export class SpecialtiesService {
  private apiUrl = `${API_BASE_URL}/specialties`;

  constructor(private http: HttpClient) {}

  getSpecialties(
    filter: SpecialtiesFilter = {},
    sort: SpecialtiesSortOptions = { field: 'name', direction: 'asc' },
    page: number = 1,
    pageSize: number = 10
  ): Observable<PaginatedResponse<Specialty>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString())
      .set('sortBy', sort.field)
      .set('sortDirection', sort.direction);

    if (filter.search) {
      params = params.set('search', filter.search);
    }
    if (filter.status) {
      params = params.set('status', filter.status);
    }

    return this.http.get<PaginatedResponse<Specialty>>(this.apiUrl, { params });
  }

  getSpecialtyById(id: string): Observable<Specialty> {
    return this.http.get<Specialty>(`${this.apiUrl}/${id}`);
  }

  createSpecialty(specialty: CreateSpecialtyDto): Observable<Specialty> {
    return this.http.post<Specialty>(this.apiUrl, specialty);
  }

  updateSpecialty(id: string, updates: UpdateSpecialtyDto): Observable<Specialty> {
    return this.http.put<Specialty>(`${this.apiUrl}/${id}`, updates);
  }

  deleteSpecialty(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  toggleSpecialtyStatus(id: string): Observable<Specialty> {
    return this.http.patch<Specialty>(`${this.apiUrl}/${id}/toggle-status`, {});
  }
}
