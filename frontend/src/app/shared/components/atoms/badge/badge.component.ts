import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

export type BadgeVariant = 'primary' | 'secondary' | 'success' | 'danger' | 'warning' | 'info';
export type BadgeSize = 'sm' | 'md' | 'lg';

@Component({
  selector: 'app-badge',
  standalone: true,
  imports: [CommonModule],
  template: `
    <span 
      class="badge" 
      [class]="'badge-' + variant + ' badge-' + size"
      [class.badge-outlined]="outlined"
    >
      <ng-content></ng-content>
    </span>
  `,
  styles: [`
    .badge {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      font-weight: 600;
      border-radius: 2rem;
      white-space: nowrap;
      transition: all 0.2s;
    }

    .badge-sm {
      padding: 0.25rem 0.75rem;
      font-size: 0.75rem;
    }

    .badge-md {
      padding: 0.5rem 1rem;
      font-size: 0.875rem;
    }

    .badge-lg {
      padding: 0.75rem 1.25rem;
      font-size: 1rem;
    }

    .badge-primary {
      background: var(--primary-600-alpha-15);
      color: var(--primary-500);
      border: 1px solid var(--primary-600-alpha-30);
    }

    .badge-secondary {
      background: var(--gray-500-alpha-15);
      color: var(--text-tertiary);
      border: 1px solid var(--gray-500-alpha-30);
    }

    .badge-success {
      background: var(--success-600-alpha-15);
      color: var(--success-400);
      border: 1px solid var(--success-600-alpha-30);
    }

    .badge-danger {
      background: var(--danger-600-alpha-15);
      color: var(--danger-400);
      border: 1px solid var(--danger-600-alpha-30);
    }

    .badge-warning {
      background: var(--warning-600-alpha-15);
      color: var(--warning-400);
      border: 1px solid var(--warning-600-alpha-30);
    }

    .badge-info {
      background: var(--cyan-500-alpha-15);
      color: var(--info-400);
      border: 1px solid var(--cyan-500-alpha-30);
    }

    .badge-outlined {
      background: transparent;
    }
  `]
})
export class BadgeComponent {
  @Input() variant: BadgeVariant = 'primary';
  @Input() size: BadgeSize = 'md';
  @Input() outlined: boolean = false;
}
