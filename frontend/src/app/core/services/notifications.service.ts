import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { AuthService } from './auth.service';
import { environment } from '@env/environment';

const API_BASE_URL = environment.apiUrl;

export type NotificationType = 'Info' | 'Warning' | 'Error' | 'Success';

export interface Notification {
  id: string;
  userId: string;
  title: string;
  message: string;
  type: NotificationType;
  isRead: boolean;
  createdAt: string;
  readAt?: string;
  link?: string;
}

export interface CreateNotificationDto {
  userId: string;
  title: string;
  message: string;
  type: NotificationType;
}

export interface NotificationsFilter {
  isRead?: boolean;
  type?: string;
}

export interface PaginatedResponse<T> {
  data: T[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

@Injectable({
  providedIn: 'root'
})
export class NotificationsService {
  private apiUrl = `${API_BASE_URL}/notificacoes`;
  private authService = inject(AuthService);

  constructor(private http: HttpClient) {}

  private mapBackendNotification(backendNotif: any): Notification {
    return {
      id: backendNotif.id,
      userId: backendNotif.usuarioId || '',
      title: backendNotif.titulo || '',
      message: backendNotif.mensagem || '',
      type: backendNotif.tipo || 'Info',
      isRead: backendNotif.lida || false,
      createdAt: backendNotif.criadoEm || new Date().toISOString(),
      link: backendNotif.link
    };
  }

  getNotifications(
    filter?: NotificationsFilter,
    page: number = 1,
    pageSize: number = 10
  ): Observable<PaginatedResponse<Notification>> {
    const user = this.authService.currentUser();
    const userId = user?.id;
    if (!userId) {
      // Return empty result instead of throwing error during SSR
      return new Observable(observer => {
        observer.next({ data: [], total: 0, page: 1, pageSize: 10, totalPages: 0 });
        observer.complete();
      });
    }

    let params = new HttpParams()
      .set('pagina', page.toString())
      .set('tamanhoPagina', pageSize.toString());

    if (filter?.isRead !== undefined) {
      params = params.set('apenasNaoLidas', (!filter.isRead).toString());
    }
    if (filter?.type) {
      params = params.set('type', filter.type);
    }

    return this.http.get<any>(this.apiUrl, { params }).pipe(
      map(response => ({
        data: (response.dados || []).map((n: any) => this.mapBackendNotification(n)),
        total: response.total || 0,
        page: response.pagina || page,
        pageSize: response.tamanhoPagina || pageSize,
        totalPages: response.totalPaginas || 0
      }))
    );
  }

  getNotificationById(id: string): Observable<Notification> {
    return this.http.get<any>(`${this.apiUrl}/${id}`).pipe(
      map(n => this.mapBackendNotification(n))
    );
  }

  getUnreadCount(): Observable<{ contagem: number }> {
    const userId = this.authService.currentUser()?.id;
    console.log('[NotificationsService] getUnreadCount chamado, userId:', userId);
    if (!userId) {
      return new Observable(observer => {
        observer.next({ contagem: 0 });
        observer.complete();
      });
    }
    console.log('[NotificationsService] Fazendo request GET /nao-lidas/contagem');
    return this.http.get<{ contagem: number }>(`${this.apiUrl}/nao-lidas/contagem`);
  }

  markAsRead(notificationId: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/${notificationId}/marcar-lida`, {});
  }

  markAllAsRead(): Observable<any> {
    const userId = this.authService.currentUser()?.id;
    if (!userId) {
      return new Observable(observer => {
        observer.next({});
        observer.complete();
      });
    }
    return this.http.post<any>(`${this.apiUrl}/marcar-todas-lidas`, {});
  }

  deleteNotification(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  createNotification(notification: CreateNotificationDto): Observable<Notification> {
    return this.http.post<Notification>(this.apiUrl, notification);
  }
}
