import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';

export type NotificationType = 'info' | 'warning' | 'error' | 'success';
export type NotificationStatus = 'read' | 'unread';

export interface Notification {
  id: string;
  type: NotificationType;
  title: string;
  message: string;
  timestamp: string;
  status: NotificationStatus;
}

export interface NotificationsFilter {
  status?: 'all' | NotificationStatus;
  type?: 'all' | NotificationType;
}

@Injectable({
  providedIn: 'root'
})
export class NotificationsService {
  // TODO: Substituir por chamadas reais ao backend
  getNotifications(filter?: NotificationsFilter): Observable<Notification[]> {
    // Dados mockados para demonstração
    const mockNotifications: Notification[] = [
      {
        id: '1',
        type: 'info',
        title: 'Nova atualização disponível',
        message: 'Uma nova versão do sistema está disponível. Atualize para obter as últimas funcionalidades.',
        timestamp: '2024-12-10T14:30:00',
        status: 'unread'
      },
      {
        id: '2',
        type: 'success',
        title: 'Backup concluído',
        message: 'O backup automático dos dados foi concluído com sucesso.',
        timestamp: '2024-12-10T12:00:00',
        status: 'read'
      },
      {
        id: '3',
        type: 'warning',
        title: 'Atenção: Limite de armazenamento',
        message: 'Você está usando 85% do espaço de armazenamento disponível.',
        timestamp: '2024-12-10T10:15:00',
        status: 'unread'
      },
      {
        id: '4',
        type: 'error',
        title: 'Falha no envio de e-mail',
        message: 'Houve um erro ao enviar notificações por e-mail. Verifique as configurações.',
        timestamp: '2024-12-09T16:45:00',
        status: 'unread'
      },
      {
        id: '5',
        type: 'info',
        title: 'Novo usuário registrado',
        message: 'Um novo profissional de saúde se registrou na plataforma.',
        timestamp: '2024-12-09T14:20:00',
        status: 'read'
      },
      {
        id: '6',
        type: 'success',
        title: 'Pagamento processado',
        message: 'O pagamento da assinatura mensal foi processado com sucesso.',
        timestamp: '2024-12-09T09:00:00',
        status: 'read'
      }
    ];

    let filtered = [...mockNotifications];

    if (filter?.status && filter.status !== 'all') {
      filtered = filtered.filter(n => n.status === filter.status);
    }

    if (filter?.type && filter.type !== 'all') {
      filtered = filtered.filter(n => n.type === filter.type);
    }

    return of(filtered).pipe(delay(300));
  }

  markAsRead(notificationId: string): Observable<void> {
    // TODO: Implementar chamada real ao backend
    return of(void 0).pipe(delay(200));
  }

  markAllAsRead(): Observable<void> {
    // TODO: Implementar chamada real ao backend
    return of(void 0).pipe(delay(200));
  }

  deleteNotification(notificationId: string): Observable<void> {
    // TODO: Implementar chamada real ao backend
    return of(void 0).pipe(delay(200));
  }
}
