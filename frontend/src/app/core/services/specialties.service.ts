import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '@env/environment';

const API_BASE_URL = environment.apiUrl;

export type SpecialtyStatus = 'Ativo' | 'Inativo';

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
  nome: string;
  descricao: string;
  status: SpecialtyStatus;
  criadoEm: string;
  atualizadoEm?: string;
  camposPersonalizados?: CustomField[];
}

export interface CreateSpecialtyDto {
  nome: string;
  descricao: string;
  status: SpecialtyStatus;
  camposPersonalizados?: CustomField[];
}

export interface UpdateSpecialtyDto {
  nome?: string;
  descricao?: string;
  status?: SpecialtyStatus;
  camposPersonalizados?: CustomField[];
}

export interface SpecialtiesFilter {
  busca?: string;
  status?: string;
}

export interface PaginatedResponse<T> {
  dados: T[];
  total: number;
  pagina: number;
  tamanhoPagina: number;
  totalPaginas: number;
}

export interface SpecialtiesSortOptions {
  field: keyof Specialty;
  direction: 'asc' | 'desc';
}

@Injectable({
  providedIn: 'root'
})
export class SpecialtiesService {
  private apiUrl = `${API_BASE_URL}/especialidades`;

  constructor(private http: HttpClient) {}

  getSpecialties(
    filter: SpecialtiesFilter = {},
    sort: SpecialtiesSortOptions = { field: 'nome', direction: 'asc' },
    page: number = 1,
    pageSize: number = 10
  ): Observable<PaginatedResponse<Specialty>> {
    let params = new HttpParams()
      .set('pagina', page.toString())
      .set('tamanhoPagina', pageSize.toString())
      .set('ordenarPor', sort.field)
      .set('direcaoOrdenacao', sort.direction);

    if (filter.busca) {
      params = params.set('busca', filter.busca);
    }
    if (filter.status) {
      params = params.set('status', filter.status);
    }

    return this.http.get<any>(this.apiUrl, { params }).pipe(
      map(response => ({
        ...response,
        data: response.data.map((s: any) => this.mapSpecialtyFromApi(s))
      }))
    );
  }

  getSpecialtyById(id: string): Observable<Specialty> {
    return this.http.get<any>(`${this.apiUrl}/${id}`).pipe(
      map(s => this.mapSpecialtyFromApi(s))
    );
  }

  createSpecialty(specialty: CreateSpecialtyDto): Observable<Specialty> {
    const payload = {
      nome: specialty.nome,
      descricao: specialty.descricao,
      camposPersonalizadosJson: specialty.camposPersonalizados && specialty.camposPersonalizados.length > 0 
        ? JSON.stringify(specialty.camposPersonalizados) 
        : null
    };
    return this.http.post<any>(this.apiUrl, payload).pipe(
      map(s => this.mapSpecialtyFromApi(s))
    );
  }

  updateSpecialty(id: string, updates: UpdateSpecialtyDto): Observable<Specialty> {
    const payload: any = {};
    if (updates.nome !== undefined) payload.nome = updates.nome;
    if (updates.descricao !== undefined) payload.descricao = updates.descricao;
    if (updates.status !== undefined) payload.status = updates.status;
    if (updates.camposPersonalizados !== undefined) {
      payload.camposPersonalizadosJson = updates.camposPersonalizados && updates.camposPersonalizados.length > 0
        ? JSON.stringify(updates.camposPersonalizados)
        : null;
    }
    return this.http.put<any>(`${this.apiUrl}/${id}`, payload).pipe(
      map(s => this.mapSpecialtyFromApi(s))
    );
  }

  deleteSpecialty(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  toggleSpecialtyStatus(id: string, currentStatus: SpecialtyStatus): Observable<Specialty> {
    const newStatus: SpecialtyStatus = currentStatus === 'Ativo' ? 'Inativo' : 'Ativo';
    return this.http.put<any>(`${this.apiUrl}/${id}`, { status: newStatus }).pipe(
      map(s => this.mapSpecialtyFromApi(s))
    );
  }

  private mapSpecialtyFromApi(apiSpecialty: any): Specialty {
    return {
      id: apiSpecialty.id,
      nome: apiSpecialty.nome,
      descricao: apiSpecialty.descricao,
      status: apiSpecialty.status,
      criadoEm: apiSpecialty.criadoEm,
      atualizadoEm: apiSpecialty.atualizadoEm,
      camposPersonalizados: apiSpecialty.camposPersonalizadosJson 
        ? JSON.parse(apiSpecialty.camposPersonalizadosJson) 
        : []
    };
  }
}
