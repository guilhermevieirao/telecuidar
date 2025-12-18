import { UserRolePipe } from './user-role.pipe';
import { UserRole } from '@app/core/services/users.service';

describe('UserRolePipe', () => {
  let pipe: UserRolePipe;

  beforeEach(() => {
    pipe = new UserRolePipe();
  });

  it('should create an instance', () => {
    expect(pipe).toBeTruthy();
  });

  it('should transform "patient" to "Paciente"', () => {
    expect(pipe.transform('PATIENT' as UserRole)).toBe('Paciente');
  });

  it('should transform "professional" to "Profissional"', () => {
    expect(pipe.transform('PROFESSIONAL' as UserRole)).toBe('Profissional');
  });

  it('should transform "admin" to "Administrador"', () => {
    expect(pipe.transform('ADMIN' as UserRole)).toBe('Administrador');
  });

  it('should return original value for unknown role', () => {
    expect(pipe.transform('unknown' as UserRole)).toBe('unknown');
  });
});
