import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

export type DividerOrientation = 'horizontal' | 'vertical';

@Component({
  selector: 'app-divider',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div 
      class="divider" 
      [class.divider-horizontal]="orientation === 'horizontal'"
      [class.divider-vertical]="orientation === 'vertical'"
      [class.divider-dashed]="dashed"
    >
      <span *ngIf="label" class="divider-label">
        {{ label }}
      </span>
    </div>
  `,
  styles: [`
    .divider {
      display: flex;
      align-items: center;
      color: var(--text-tertiary);
      font-size: 0.875rem;
    }

    .divider-horizontal {
      width: 100%;
      border-top: 1px solid var(--border-primary);
      margin: 1rem 0;
    }

    .divider-vertical {
      height: 100%;
      border-left: 1px solid var(--border-primary);
      margin: 0 1rem;
      min-height: 1rem;
    }

    .divider-dashed.divider-horizontal {
      border-top-style: dashed;
    }

    .divider-dashed.divider-vertical {
      border-left-style: dashed;
    }

    .divider-label {
      padding: 0 0.75rem;
      background: var(--card-bg);
      position: relative;
      top: -0.5px;
    }

    .divider-horizontal:has(.divider-label) {
      justify-content: center;
    }

  `]
})
export class DividerComponent {
  @Input() orientation: DividerOrientation = 'horizontal';
  @Input() dashed: boolean = false;
  @Input() label?: string;
}
