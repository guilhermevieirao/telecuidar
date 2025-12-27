import { Pipe, PipeTransform } from '@angular/core';
import { StatusUsuario } from '@app/core/services/users.service';

@Pipe({
  name: 'userStatus'
})
export class UserStatusPipe implements PipeTransform {
  transform(status: StatusUsuario | string): string {
    const statusMap: Record<string, string> = {
      'Ativo': 'Ativo',
      'Inativo': 'Inativo',
      'Active': 'Ativo',
      'Inactive': 'Inativo'
    };
    return statusMap[status] || status;
  }
}
