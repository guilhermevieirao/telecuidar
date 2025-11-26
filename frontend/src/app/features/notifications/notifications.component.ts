import { Component, OnInit, OnDestroy, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { interval, Subscription } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { Router } from '@angular/router';

interface Notification {
  id: number;
  title: string;
  message: string;
  type: 'info' | 'success' | 'warning' | 'error';
  actionUrl?: string;
  actionText?: string;
  isRead: boolean;
  timeAgo: string;
  createdAt: string;
}

interface NotificationsResponse {
  data: Notification[];
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  totalCount: number;
}

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.scss']
})
export class NotificationsComponent implements OnInit, OnDestroy {
  private apiUrl = `${environment.apiUrl}/notifications`;
  
  notifications = signal<Notification[]>([]);
  unreadCount = signal<number>(0);
  isOpen = signal<boolean>(false);
  isLoading = signal<boolean>(false);
  showOnlyUnread = signal<boolean>(false);
  
  pageNumber = 1;
  totalPages = 1;
  hasMore = false;
  
  private pollSubscription?: Subscription;

  constructor(
    private http: HttpClient,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadNotifications();
    this.loadUnreadCount();
    
    // Poll para novas notificações a cada 30 segundos
    this.pollSubscription = interval(30000)
      .pipe(switchMap(() => this.http.get<{ count: number }>(`${this.apiUrl}/unread-count`)))
      .subscribe({
        next: (response) => {
          const newCount = response.count;
          if (newCount !== this.unreadCount()) {
            this.unreadCount.set(newCount);
            if (this.isOpen()) {
              this.loadNotifications();
            }
          }
        },
        error: (error) => console.error('Erro ao atualizar contador:', error)
      });
  }

  ngOnDestroy() {
    this.pollSubscription?.unsubscribe();
  }

  toggleDropdown() {
    this.isOpen.update(value => !value);
    if (this.isOpen()) {
      this.loadNotifications();
    }
  }

  closeDropdown() {
    this.isOpen.set(false);
  }

  loadUnreadCount() {
    this.http.get<{ count: number }>(`${this.apiUrl}/unread-count`).subscribe({
      next: (response) => this.unreadCount.set(response.count),
      error: (error) => console.error('Erro ao carregar contador:', error)
    });
  }

  loadNotifications() {
    this.isLoading.set(true);
    
    const params: any = {
      pageNumber: this.pageNumber,
      pageSize: 10
    };
    
    if (this.showOnlyUnread()) {
      params.onlyUnread = true;
    }

    this.http.get<NotificationsResponse>(this.apiUrl, { params }).subscribe({
      next: (response) => {
        if (this.pageNumber === 1) {
          this.notifications.set(response.data);
        } else {
          this.notifications.update(current => [...current, ...response.data]);
        }
        this.totalPages = response.totalPages;
        this.hasMore = this.pageNumber < response.totalPages;
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Erro ao carregar notificações:', error);
        this.isLoading.set(false);
      }
    });
  }

  loadMore() {
    if (this.hasMore && !this.isLoading()) {
      this.pageNumber++;
      this.loadNotifications();
    }
  }

  toggleFilter() {
    this.showOnlyUnread.update(value => !value);
    this.pageNumber = 1;
    this.loadNotifications();
  }

  markAsRead(notification: Notification, event?: Event) {
    if (event) {
      event.stopPropagation();
    }
    
    if (notification.isRead) return;

    this.http.patch(`${this.apiUrl}/${notification.id}/mark-as-read`, {}).subscribe({
      next: () => {
        // Atualizar localmente
        this.notifications.update(current =>
          current.map(n => n.id === notification.id ? { ...n, isRead: true } : n)
        );
        this.unreadCount.update(count => Math.max(0, count - 1));
      },
      error: (error) => console.error('Erro ao marcar como lida:', error)
    });
  }

  handleNotificationClick(notification: Notification) {
    this.markAsRead(notification);
    
    if (notification.actionUrl) {
      this.closeDropdown();
      this.router.navigateByUrl(notification.actionUrl);
    }
  }

  getIcon(type: string): string {
    const icons: Record<string, string> = {
      info: '📢',
      success: '✅',
      warning: '⚠️',
      error: '❌'
    };
    return icons[type] || '📢';
  }

  getTypeClass(type: string): string {
    const classes: Record<string, string> = {
      info: 'notification-info',
      success: 'notification-success',
      warning: 'notification-warning',
      error: 'notification-error'
    };
    return classes[type] || 'notification-info';
  }
}
