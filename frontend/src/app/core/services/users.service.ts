import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay, map } from 'rxjs/operators';

export type UserRole = 'patient' | 'professional' | 'admin';
export type UserStatus = 'active' | 'inactive';

export interface User {
  id: string;
  name: string;
  email: string;
  role: UserRole;
  cpf: string;
  phone: string;
  status: UserStatus;
  createdAt: string;
  avatar?: string;
  specialtyId?: string;
  emailVerified?: boolean;
}

export interface UsersFilter {
  search?: string;
  role?: UserRole | 'all';
  status?: UserStatus | 'all';
}

export interface PaginatedResponse<T> {
  data: T[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface UsersSortOptions {
  field: keyof User;
  direction: 'asc' | 'desc';
}

@Injectable({
  providedIn: 'root'
})
export class UsersService {
  private mockUsers: User[] = [
    {
      id: '1',
      name: 'João Silva',
      email: 'joao.silva@email.com',
      role: 'patient',
      cpf: '123.456.789-00',
      phone: '(11) 98765-4321',
      status: 'active',
      createdAt: '2024-01-15T10:00:00',
      emailVerified: true
    },
    {
      id: '2',
      name: 'Maria Santos',
      email: 'maria.santos@email.com',
      role: 'professional',
      cpf: '234.567.890-11',
      phone: '(11) 98765-4322',
      status: 'active',
      createdAt: '2024-02-20T14:30:00',
      specialtyId: '1',
      emailVerified: true
    },
    {
      id: '3',
      name: 'Pedro Costa',
      email: 'pedro.costa@email.com',
      role: 'admin',
      cpf: '345.678.901-22',
      phone: '(11) 98765-4323',
      status: 'active',
      createdAt: '2024-03-10T09:15:00',
      emailVerified: false
    },
    {
      id: '4',
      name: 'Ana Oliveira',
      email: 'ana.oliveira@email.com',
      role: 'patient',
      cpf: '456.789.012-33',
      phone: '(11) 98765-4324',
      status: 'inactive',
      createdAt: '2024-04-05T16:45:00',
      emailVerified: true
    },
    {
      id: '5',
      name: 'Carlos Ferreira',
      email: 'carlos.ferreira@email.com',
      role: 'professional',
      cpf: '567.890.123-44',
      phone: '(11) 98765-4325',
      status: 'active',
      createdAt: '2024-05-12T11:20:00',
      emailVerified: true
    },
    {
      id: '6',
      name: 'Juliana Lima',
      email: 'juliana.lima@email.com',
      role: 'patient',
      cpf: '678.901.234-55',
      phone: '(11) 98765-4326',
      status: 'active',
      createdAt: '2024-06-18T13:00:00',
      emailVerified: false
    },
    {
      id: '7',
      name: 'Roberto Alves',
      email: 'roberto.alves@email.com',
      role: 'professional',
      cpf: '789.012.345-66',
      phone: '(11) 98765-4327',
      status: 'inactive',
      createdAt: '2024-07-22T15:30:00',
      emailVerified: true
    },
    {
      id: '8',
      name: 'Fernanda Souza',
      email: 'fernanda.souza@email.com',
      role: 'patient',
      cpf: '890.123.456-77',
      phone: '(11) 98765-4328',
      status: 'active',
      createdAt: '2024-08-08T10:45:00',
      emailVerified: true
    },
    {
      id: '9',
      name: 'Lucas Martins',
      email: 'lucas.martins@email.com',
      role: 'admin',
      cpf: '901.234.567-88',
      phone: '(11) 98765-4329',
      status: 'active',
      createdAt: '2024-09-14T12:00:00',
      emailVerified: false
    },
    {
      id: '10',
      name: 'Beatriz Rodrigues',
      email: 'beatriz.rodrigues@email.com',
      role: 'professional',
      cpf: '012.345.678-99',
      phone: '(11) 98765-4330',
      status: 'active',
      createdAt: '2024-10-20T14:15:00',
      emailVerified: true
    }
  ];

  // TODO: Substituir por chamadas reais ao backend
  getUsers(
    filter?: UsersFilter,
    sort?: UsersSortOptions,
    page: number = 1,
    pageSize: number = 10
  ): Observable<PaginatedResponse<User>> {
    let filtered = [...this.mockUsers];

    // Aplicar filtros
    if (filter?.search) {
      const search = filter.search.toLowerCase();
      filtered = filtered.filter(user =>
        user.name.toLowerCase().includes(search) ||
        user.email.toLowerCase().includes(search) ||
        user.cpf.includes(search) ||
        user.id.includes(search)
      );
    }

    if (filter?.role && filter.role !== 'all') {
      filtered = filtered.filter(user => user.role === filter.role);
    }

    if (filter?.status && filter.status !== 'all') {
      filtered = filtered.filter(user => user.status === filter.status);
    }

    // Aplicar ordenação
    if (sort) {
      filtered.sort((a, b) => {
        const aValue = a[sort.field];
        const bValue = b[sort.field];
        
        if (aValue === undefined || bValue === undefined) return 0;
        if (aValue < bValue) return sort.direction === 'asc' ? -1 : 1;
        if (aValue > bValue) return sort.direction === 'asc' ? 1 : -1;
        return 0;
      });
    }

    // Aplicar paginação
    const total = filtered.length;
    const totalPages = Math.ceil(total / pageSize);
    const startIndex = (page - 1) * pageSize;
    const endIndex = startIndex + pageSize;
    const data = filtered.slice(startIndex, endIndex);

    return of({
      data,
      total,
      page,
      pageSize,
      totalPages
    }).pipe(delay(300));
  }

  getUserById(id: string): Observable<User | undefined> {
    const user = this.mockUsers.find(u => u.id === id);
    return of(user).pipe(delay(200));
  }

  createUser(user: Omit<User, 'id' | 'createdAt'>): Observable<User> {
    // TODO: Implementar chamada real ao backend
    const newUser: User = {
      ...user,
      id: String(this.mockUsers.length + 1),
      createdAt: new Date().toISOString()
    };
    return of(newUser).pipe(delay(300));
  }

  updateUser(id: string, user: Partial<User>): Observable<User> {
    // TODO: Implementar chamada real ao backend
    const index = this.mockUsers.findIndex(u => u.id === id);
    if (index !== -1) {
      this.mockUsers[index] = { ...this.mockUsers[index], ...user };
      return of(this.mockUsers[index]).pipe(delay(300));
    }
    throw new Error('User not found');
  }

  deleteUser(id: string): Observable<void> {
    // TODO: Implementar chamada real ao backend
    return of(void 0).pipe(delay(300));
  }
}
