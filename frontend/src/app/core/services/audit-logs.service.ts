import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay, map } from 'rxjs/operators';

export type AuditActionType = 'create' | 'update' | 'delete' | 'login' | 'logout' | 'view' | 'export';

export interface AuditLog {
  id: string;
  userId: string;
  userName: string;
  userRole: string;
  action: AuditActionType;
  entityType: string; // user, specialty, appointment, etc
  entityId?: string;
  description: string;
  ipAddress: string;
  userAgent: string;
  timestamp: string;
}

export interface AuditLogsFilter {
  search?: string;
  action?: AuditActionType | 'all';
  entityType?: string | 'all';
  userId?: string;
  startDate?: string;
  endDate?: string;
}

export interface PaginatedResponse<T> {
  data: T[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface AuditLogsSortOptions {
  field: keyof AuditLog;
  direction: 'asc' | 'desc';
}

@Injectable({
  providedIn: 'root'
})
export class AuditLogsService {
  private mockLogs: AuditLog[] = [
    {
      id: '1',
      userId: '1',
      userName: 'Pedro Costa',
      userRole: 'Administrador',
      action: 'create',
      entityType: 'user',
      entityId: '10',
      description: 'Criou novo usuário: Maria Silva',
      ipAddress: '192.168.1.100',
      userAgent: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/120.0.0.0',
      timestamp: '2024-12-10T14:30:00'
    },
    {
      id: '2',
      userId: '1',
      userName: 'Pedro Costa',
      userRole: 'Administrador',
      action: 'update',
      entityType: 'specialty',
      entityId: '2',
      description: 'Atualizou especialidade: Dermatologia',
      ipAddress: '192.168.1.100',
      userAgent: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/120.0.0.0',
      timestamp: '2024-12-10T13:15:00'
    },
    {
      id: '3',
      userId: '2',
      userName: 'Maria Santos',
      userRole: 'Profissional',
      action: 'login',
      entityType: 'auth',
      description: 'Login realizado com sucesso',
      ipAddress: '192.168.1.105',
      userAgent: 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) Safari/537.36',
      timestamp: '2024-12-10T12:45:00'
    },
    {
      id: '4',
      userId: '1',
      userName: 'Pedro Costa',
      userRole: 'Administrador',
      action: 'delete',
      entityType: 'user',
      entityId: '8',
      description: 'Removeu usuário: João Teste',
      ipAddress: '192.168.1.100',
      userAgent: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/120.0.0.0',
      timestamp: '2024-12-10T11:20:00'
    },
    {
      id: '5',
      userId: '3',
      userName: 'João Silva',
      userRole: 'Paciente',
      action: 'view',
      entityType: 'appointment',
      entityId: '15',
      description: 'Visualizou detalhes da consulta',
      ipAddress: '192.168.1.110',
      userAgent: 'Mozilla/5.0 (iPhone; CPU iPhone OS 16_0 like Mac OS X) Safari/604.1',
      timestamp: '2024-12-10T10:30:00'
    },
    {
      id: '6',
      userId: '1',
      userName: 'Pedro Costa',
      userRole: 'Administrador',
      action: 'export',
      entityType: 'report',
      description: 'Exportou relatório de usuários',
      ipAddress: '192.168.1.100',
      userAgent: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/120.0.0.0',
      timestamp: '2024-12-10T09:00:00'
    },
    {
      id: '7',
      userId: '2',
      userName: 'Maria Santos',
      userRole: 'Profissional',
      action: 'update',
      entityType: 'user',
      entityId: '2',
      description: 'Atualizou seu perfil',
      ipAddress: '192.168.1.105',
      userAgent: 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) Safari/537.36',
      timestamp: '2024-12-10T08:45:00'
    },
    {
      id: '8',
      userId: '1',
      userName: 'Pedro Costa',
      userRole: 'Administrador',
      action: 'create',
      entityType: 'specialty',
      entityId: '9',
      description: 'Criou nova especialidade: Endocrinologia',
      ipAddress: '192.168.1.100',
      userAgent: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/120.0.0.0',
      timestamp: '2024-12-09T16:30:00'
    },
    {
      id: '9',
      userId: '4',
      userName: 'Ana Paula',
      userRole: 'Profissional',
      action: 'logout',
      entityType: 'auth',
      description: 'Logout realizado',
      ipAddress: '192.168.1.112',
      userAgent: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) Firefox/120.0',
      timestamp: '2024-12-09T15:00:00'
    },
    {
      id: '10',
      userId: '1',
      userName: 'Pedro Costa',
      userRole: 'Administrador',
      action: 'update',
      entityType: 'user',
      entityId: '5',
      description: 'Alterou status do usuário: Carlos Souza para inativo',
      ipAddress: '192.168.1.100',
      userAgent: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/120.0.0.0',
      timestamp: '2024-12-09T14:15:00'
    },
    {
      id: '11',
      userId: '2',
      userName: 'Maria Santos',
      userRole: 'Profissional',
      action: 'view',
      entityType: 'patient',
      entityId: '20',
      description: 'Visualizou prontuário do paciente',
      ipAddress: '192.168.1.105',
      userAgent: 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) Safari/537.36',
      timestamp: '2024-12-09T13:30:00'
    },
    {
      id: '12',
      userId: '1',
      userName: 'Pedro Costa',
      userRole: 'Administrador',
      action: 'create',
      entityType: 'user',
      entityId: '11',
      description: 'Criou novo usuário: Roberto Lima',
      ipAddress: '192.168.1.100',
      userAgent: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/120.0.0.0',
      timestamp: '2024-12-09T12:00:00'
    },
    {
      id: '13',
      userId: '3',
      userName: 'João Silva',
      userRole: 'Paciente',
      action: 'login',
      entityType: 'auth',
      description: 'Login realizado com sucesso',
      ipAddress: '192.168.1.110',
      userAgent: 'Mozilla/5.0 (iPhone; CPU iPhone OS 16_0 like Mac OS X) Safari/604.1',
      timestamp: '2024-12-09T11:00:00'
    },
    {
      id: '14',
      userId: '1',
      userName: 'Pedro Costa',
      userRole: 'Administrador',
      action: 'export',
      entityType: 'report',
      description: 'Exportou relatório de especialidades',
      ipAddress: '192.168.1.100',
      userAgent: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/120.0.0.0',
      timestamp: '2024-12-09T10:30:00'
    },
    {
      id: '15',
      userId: '2',
      userName: 'Maria Santos',
      userRole: 'Profissional',
      action: 'update',
      entityType: 'appointment',
      entityId: '18',
      description: 'Atualizou dados da consulta',
      ipAddress: '192.168.1.105',
      userAgent: 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) Safari/537.36',
      timestamp: '2024-12-09T09:15:00'
    }
  ];

  getAuditLogs(
    filter: AuditLogsFilter = {},
    sort: AuditLogsSortOptions = { field: 'timestamp', direction: 'desc' },
    page: number = 1,
    pageSize: number = 10
  ): Observable<PaginatedResponse<AuditLog>> {
    return of(this.mockLogs).pipe(
      delay(500),
      map(logs => {
        let filtered = [...logs];

        if (filter.search) {
          const searchLower = filter.search.toLowerCase();
          filtered = filtered.filter(log =>
            log.userName.toLowerCase().includes(searchLower) ||
            log.description.toLowerCase().includes(searchLower) ||
            log.entityType.toLowerCase().includes(searchLower) ||
            log.ipAddress.includes(searchLower)
          );
        }

        if (filter.action && filter.action !== 'all') {
          filtered = filtered.filter(log => log.action === filter.action);
        }

        if (filter.entityType && filter.entityType !== 'all') {
          filtered = filtered.filter(log => log.entityType === filter.entityType);
        }

        if (filter.userId) {
          filtered = filtered.filter(log => log.userId === filter.userId);
        }

        if (filter.startDate) {
          filtered = filtered.filter(log => log.timestamp >= filter.startDate!);
        }

        if (filter.endDate) {
          filtered = filtered.filter(log => log.timestamp <= filter.endDate!);
        }

        const sorted = filtered.sort((a, b) => {
          const aValue = a[sort.field];
          const bValue = b[sort.field];
          
          if (aValue === undefined || bValue === undefined) return 0;
          if (aValue < bValue) return sort.direction === 'asc' ? -1 : 1;
          if (aValue > bValue) return sort.direction === 'asc' ? 1 : -1;
          return 0;
        });

        const startIndex = (page - 1) * pageSize;
        const endIndex = startIndex + pageSize;
        const paginatedData = sorted.slice(startIndex, endIndex);

        return {
          data: paginatedData,
          total: sorted.length,
          page,
          pageSize,
          totalPages: Math.ceil(sorted.length / pageSize)
        };
      })
    );
  }

  exportToPDF(filter: AuditLogsFilter = {}): Observable<Blob> {
    // Simular exportação PDF
    return of(new Blob(['PDF content'], { type: 'application/pdf' })).pipe(delay(1000));
  }
}
