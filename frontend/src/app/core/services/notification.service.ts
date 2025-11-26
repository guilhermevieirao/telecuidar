import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface CreateNotificationRequest {
  title: string;
  message: string;
  type: 'info' | 'success' | 'warning' | 'error';
  userId?: number;
  actionUrl?: string;
  actionText?: string;
  relatedEntityType?: string;
  relatedEntityId?: number;
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private apiUrl = `${environment.apiUrl}/notifications`;

  constructor(private http: HttpClient) {}

  /**
   * Cria uma notificação (apenas admins)
   */
  createNotification(request: CreateNotificationRequest): Observable<any> {
    return this.http.post(this.apiUrl, request);
  }

  /**
   * Busca notificações do usuário atual
   */
  getMyNotifications(pageNumber: number = 1, onlyUnread: boolean = false): Observable<any> {
    const params: any = { pageNumber, pageSize: 10 };
    if (onlyUnread) {
      params.onlyUnread = true;
    }
    return this.http.get(this.apiUrl, { params });
  }

  /**
   * Obtém contagem de notificações não lidas
   */
  getUnreadCount(): Observable<{ count: number }> {
    return this.http.get<{ count: number }>(`${this.apiUrl}/unread-count`);
  }

  /**
   * Marca notificação como lida
   */
  markAsRead(notificationId: number): Observable<any> {
    return this.http.patch(`${this.apiUrl}/${notificationId}/mark-as-read`, {});
  }

  /**
   * Helpers para criar notificações específicas (apenas admin)
   */
  
  notifyFileUploaded(userId: number, fileName: string, fileId: number): Observable<any> {
    return this.createNotification({
      title: 'Novo arquivo enviado',
      message: `O arquivo "${fileName}" foi enviado com sucesso.`,
      type: 'success',
      userId,
      actionUrl: '/arquivos',
      actionText: 'Ver arquivos',
      relatedEntityType: 'FileUpload',
      relatedEntityId: fileId
    });
  }

  notifyAppointmentReminder(userId: number, appointmentDate: string): Observable<any> {
    return this.createNotification({
      title: 'Lembrete de consulta',
      message: `Você tem uma consulta agendada para ${appointmentDate}.`,
      type: 'warning',
      userId,
      actionUrl: '/dashboard',
      actionText: 'Ver consultas'
    });
  }

  notifyNewMessage(userId: number, senderName: string): Observable<any> {
    return this.createNotification({
      title: 'Nova mensagem',
      message: `Você recebeu uma nova mensagem de ${senderName}.`,
      type: 'info',
      userId,
      actionUrl: '/dashboard',
      actionText: 'Ver mensagens'
    });
  }

  notifyAccountStatus(userId: number, isActive: boolean): Observable<any> {
    return this.createNotification({
      title: isActive ? 'Conta ativada' : 'Conta desativada',
      message: isActive 
        ? 'Sua conta foi ativada com sucesso.' 
        : 'Sua conta foi temporariamente desativada.',
      type: isActive ? 'success' : 'warning',
      userId,
      actionUrl: '/perfil',
      actionText: 'Ver perfil'
    });
  }

  notifyWelcome(userId: number, userName: string): Observable<any> {
    return this.createNotification({
      title: 'Bem-vindo ao TeleCuidar! 🎉',
      message: `Olá ${userName}! Sua conta foi criada com sucesso. Explore os recursos da plataforma.`,
      type: 'success',
      userId,
      actionUrl: '/dashboard',
      actionText: 'Ir para o painel'
    });
  }

  notifySystemMaintenance(userId: number, maintenanceDate: string): Observable<any> {
    return this.createNotification({
      title: '⚙️ Manutenção programada',
      message: `O sistema entrará em manutenção em ${maintenanceDate}. Salve seu trabalho.`,
      type: 'warning',
      userId
    });
  }

  notifyError(userId: number, errorMessage: string): Observable<any> {
    return this.createNotification({
      title: 'Erro no sistema',
      message: errorMessage,
      type: 'error',
      userId,
      actionUrl: '/perfil',
      actionText: 'Contatar suporte'
    });
  }
}
