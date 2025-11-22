import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'phone',
  standalone: true
})
export class PhonePipe implements PipeTransform {
  transform(value: string | number): string {
    if (!value) return '';
    
    const phone = value.toString().replace(/\D/g, '');
    
    // Celular com DDD (11 dígitos): (99) 99999-9999
    if (phone.length === 11) {
      return phone.replace(/(\d{2})(\d{5})(\d{4})/, '($1) $2-$3');
    }
    
    // Telefone fixo com DDD (10 dígitos): (99) 9999-9999
    if (phone.length === 10) {
      return phone.replace(/(\d{2})(\d{4})(\d{4})/, '($1) $2-$3');
    }
    
    return value.toString();
  }
}
