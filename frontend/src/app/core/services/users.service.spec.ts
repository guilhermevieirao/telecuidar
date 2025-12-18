import { TestBed } from '@angular/core/testing';
import { UsersService, UserRole, UserStatus } from './users.service';
import { firstValueFrom } from 'rxjs';

describe('UsersService', () => {
  let service: UsersService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UsersService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getUsers', () => {
    it('should return paginated users', async () => {
      const result = await firstValueFrom(service.getUsers({}, 1, 10));
      
      expect(result).toBeDefined();
      expect(result.data).toBeInstanceOf(Array);
      expect(result.total).toBeGreaterThan(0);
      expect(result.page).toBe(1);
      expect(result.pageSize).toBe(10);
    });

    it('should filter by role', async () => {
      const result = await firstValueFrom(
        service.getUsers({ role: 'PATIENT' }, 1, 10)
      );
      
      expect(result.data.every(user => user.role === 'PATIENT')).toBe(true);
    });

    it('should filter by status', async () => {
      const result = await firstValueFrom(
        service.getUsers({ status: 'Active' }, 1, 10)
      );
      
      expect(result.data.every(user => user.status === 'Active')).toBe(true);
    });

    it('should search by name or email', async () => {
      const result = await firstValueFrom(
        service.getUsers({ search: 'silva' }, 1, 10)
      );
      
      expect(result.data.length).toBeGreaterThan(0);
      expect(result.data.some(user => 
        user.name.toLowerCase().includes('silva') || 
        user.email.toLowerCase().includes('silva')
      )).toBe(true);
    });
  });

  describe('getUserById', () => {
    it('should return user by id', async () => {
      const user = await firstValueFrom(service.getUserById('1'));
      
      expect(user).toBeDefined();
      expect(user?.id).toBe('1');
    });

    it('should return undefined for non-existent id', async () => {
      const user = await firstValueFrom(service.getUserById('999999'));
      
      expect(user).toBeUndefined();
    });
  });

  describe('createUser', () => {
    it('should create a new user', async () => {
      const newUser = {
        name: 'Novo',
        lastName: 'Usuário',
        email: 'novo@test.com',
        password: 'Test123!',
        role: 'PATIENT' as UserRole,
        cpf: '999.888.777-66',
        phone: '(11) 99999-8888',
        status: 'Active' as UserStatus
      };

      const created = await firstValueFrom(service.createUser(newUser));
      
      expect(created).toBeDefined();
      expect(created.id).toBeDefined();
      expect(created.name).toBe(newUser.name);
      expect(created.email).toBe(newUser.email);
    });
  });

  describe('updateUser', () => {
    it('should update an existing user', async () => {
      const updates = { name: 'Nome Atualizado' };
      
      const updated = await firstValueFrom(service.updateUser('1', updates));
      
      expect(updated).toBeDefined();
      expect(updated.name).toBe('Nome Atualizado');
    });
  });

  describe('deleteUser', () => {
    it('should delete a user', async () => {
      const result = await firstValueFrom(service.deleteUser('1'));
      
      expect(result).toBeUndefined();
    });
  });

});
