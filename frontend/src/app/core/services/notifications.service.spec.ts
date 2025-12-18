import { TestBed } from '@angular/core/testing';
import { NotificationsService } from './notifications.service';
import { firstValueFrom } from 'rxjs';

describe('NotificationsService', () => {
  let service: NotificationsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(NotificationsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getNotifications', () => {
    it('should return all notifications', async () => {
      const result = await firstValueFrom(service.getNotifications());
      
      expect(result).toBeDefined();
      expect(result.data).toBeInstanceOf(Array);
      expect(result.data.length).toBeGreaterThan(0);
    });

    it('should filter by isRead', async () => {
      const result = await firstValueFrom(
        service.getNotifications({ isRead: false })
      );
      
      expect(result.data.every((n: any) => n.isRead === false)).toBe(true);
    });

    it('should filter by type', async () => {
      const result = await firstValueFrom(
        service.getNotifications({ type: 'Info' })
      );
      
      expect(result.data.every((n: any) => n.type === 'Info')).toBe(true);
    });

    it('should return all when no filter', async () => {
      const result = await firstValueFrom(
        service.getNotifications()
      );
      
      expect(result.data.length).toBeGreaterThan(0);
    });
  });

  describe('markAsRead', () => {
    it('should mark notification as read', async () => {
      const result = await firstValueFrom(service.markAsRead('1'));
      
      expect(result).toBeUndefined();
    });
  });

  describe('markAllAsRead', () => {
    it('should mark all notifications as read', async () => {
      const result = await firstValueFrom(service.markAllAsRead());
      
      expect(result).toBeUndefined();
    });
  });
});
