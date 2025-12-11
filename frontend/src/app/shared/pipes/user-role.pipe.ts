import { Pipe, PipeTransform } from '@angular/core';
import { UserRole } from '../../core/services/users.service';

@Pipe({
  name: 'userRole'
})
export class UserRolePipe implements PipeTransform {
  transform(role: UserRole): string {
    const roleMap: Record<UserRole, string> = {
      patient: 'Paciente',
      professional: 'Profissional',
      admin: 'Administrador'
    };
    return roleMap[role] || role;
  }
}
