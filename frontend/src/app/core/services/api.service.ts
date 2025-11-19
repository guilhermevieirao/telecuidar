import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

export interface ApiResponse<T> {
  data?: T;
  isSuccess: boolean;
  message: string;
  errors: string[];
}

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private readonly baseUrl = 'http://localhost:5000/api';

  constructor(private http: HttpClient) {}

  private getHeaders(): HttpHeaders {
    return new HttpHeaders({
      'Content-Type': 'application/json'
    });
  }

  get<T>(endpoint: string): Observable<T> {
    return this.http.get<ApiResponse<T>>(`${this.baseUrl}/${endpoint}`, {
      headers: this.getHeaders()
    }).pipe(
      map(response => {
        if (response.isSuccess && response.data) {
          return response.data;
        }
        throw new Error(response.message || 'Erro na requisição');
      }),
      catchError(this.handleError)
    );
  }

  post<T>(endpoint: string, body: any): Observable<T> {
    return this.http.post<ApiResponse<T>>(`${this.baseUrl}/${endpoint}`, body, {
      headers: this.getHeaders()
    }).pipe(
      map(response => {
        if (response.isSuccess && response.data) {
          return response.data;
        }
        throw new Error(response.message || 'Erro na requisição');
      }),
      catchError(this.handleError)
    );
  }

  private handleError(error: any): Observable<never> {
    console.error('Erro na API:', error);
    return throwError(() => new Error(error.message || 'Erro desconhecido'));
  }
}