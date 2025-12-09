import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="empty-state">
      <div class="empty-state-icon">{{ icon }}</div>
      <h3 class="empty-state-title">{{ title }}</h3>
      <p *ngIf="description" class="empty-state-description">
        {{ description }}
      </p>
      <div *ngIf="hasAction" class="empty-state-action">
        <ng-content></ng-content>
      </div>
    </div>
  `,
  styles: [`
    .empty-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 3rem 1.5rem;
      text-align: center;
      color: var(--text-secondary);
    }

    .empty-state-icon {
      font-size: 4rem;
      margin-bottom: 1rem;
      opacity: 0.5;
    }

    .empty-state-title {
      font-size: 1.125rem;
      font-weight: 600;
      color: var(--text-secondary);
      margin: 0 0 0.5rem 0;
    }

    .empty-state-description {
      font-size: 0.875rem;
      color: var(--text-secondary);
      max-width: 28rem;
      margin: 0 0 1.5rem 0;
      line-height: 1.5;
    }

    .empty-state-action {
      margin-top: 1rem;
    }

  `]
})
export class EmptyStateComponent {
  @Input() icon: string = '📭';
  @Input() title: string = 'Nenhum item encontrado';
  @Input() description?: string;
  @Input() hasAction: boolean = false;
}
