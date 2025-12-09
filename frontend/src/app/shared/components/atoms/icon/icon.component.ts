import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

export type IconSize = 'xs' | 'sm' | 'md' | 'lg' | 'xl';

@Component({
  selector: 'app-icon',
  standalone: true,
  imports: [CommonModule],
  template: `
    <span 
      class="icon" 
      [class]="'icon-' + size"
      [style.color]="color"
    >
      {{ icon }}
    </span>
  `,
  styles: [`
    .icon {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      line-height: 1;
    }

    .icon-xs {
      font-size: 0.75rem;
    }

    .icon-sm {
      font-size: 1rem;
    }

    .icon-md {
      font-size: 1.5rem;
    }

    .icon-lg {
      font-size: 2rem;
    }

    .icon-xl {
      font-size: 3rem;
    }
  `]
})
export class IconComponent {
  @Input() icon: string = '';
  @Input() size: IconSize = 'md';
  @Input() color?: string;
}
