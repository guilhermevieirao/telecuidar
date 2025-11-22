import { Directive, ElementRef, HostListener, Input } from '@angular/core';

@Directive({
  selector: '[appNumbersOnly]',
  standalone: true
})
export class NumbersOnlyDirective {
  @Input() allowDecimals = false;
  @Input() maxLength?: number;

  constructor(private el: ElementRef) {}

  @HostListener('input', ['$event'])
  onInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    let value = input.value;
    
    if (this.allowDecimals) {
      // Permite apenas números e um ponto decimal
      value = value.replace(/[^0-9.]/g, '');
      
      // Garante apenas um ponto decimal
      const parts = value.split('.');
      if (parts.length > 2) {
        value = parts[0] + '.' + parts.slice(1).join('');
      }
    } else {
      // Permite apenas números
      value = value.replace(/\D/g, '');
    }
    
    if (this.maxLength && value.length > this.maxLength) {
      value = value.substring(0, this.maxLength);
    }
    
    input.value = value;
  }

  @HostListener('keypress', ['$event'])
  onKeyPress(event: KeyboardEvent): boolean {
    const charCode = event.which ? event.which : event.keyCode;
    
    // Permite: backspace, delete, tab, escape, enter
    if ([8, 9, 27, 13].includes(charCode)) {
      return true;
    }
    
    // Permite ponto decimal se configurado
    if (this.allowDecimals && charCode === 46) {
      const input = event.target as HTMLInputElement;
      return !input.value.includes('.');
    }
    
    // Permite apenas números (0-9)
    if (charCode >= 48 && charCode <= 57) {
      return true;
    }
    
    event.preventDefault();
    return false;
  }
}
