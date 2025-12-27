import { Pipe, PipeTransform } from '@angular/core';
import { SpecialtyStatus } from '@app/core/services/specialties.service';

@Pipe({
  name: 'specialtyStatus'
})
export class SpecialtyStatusPipe implements PipeTransform {
  transform(status: SpecialtyStatus): string {
    const statusMap: Record<SpecialtyStatus, string> = {
      Ativo: 'Ativa',
      Inativo: 'Inativa'
    };
    return statusMap[status] || status;
  }
}
