import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BadgeComponent } from '../../atoms/badge/badge.component';
import { DividerComponent } from '../../atoms/divider/divider.component';

export type AlertType = 'info' | 'success' | 'warning' | 'danger';

@Component({
  selector: 'app-alert',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="alert" [class]="'alert-' + type" *ngIf="!isDismissed">
      <div class="alert-icon">{{ getIcon() }}</div>
      
      <div class="alert-content">
        <div *ngIf="title" class="alert-title">{{ title }}</div>
        <div class="alert-message">
          <ng-content></ng-content>
        </div>
      </div>
      
      <button 
        *ngIf="dismissible"
        type="button"
        class="alert-close"
        (click)="onDismiss()"
        aria-label="Fechar alerta"
      >
        ✕
      </button>
    </div>
  `,
  styles: [`
    .alert {
      display: flex;
      align-items: flex-start;
      gap: 0.75rem;
      padding: 1rem;
      border-radius: 0.5rem;
      border: 1px solid;
    }

    .alert-icon {
      font-size: 1.25rem;
      flex-shrink: 0;
      margin-top: 0.125rem;
    }

    .alert-content {
      flex: 1;
      min-width: 0;
    }

    .alert-title {
      font-weight: 600;
      font-size: 0.875rem;
      margin-bottom: 0.25rem;
    }

    .alert-message {
      font-size: 0.875rem;
      line-height: 1.5;
    }

    .alert-close {
      padding: 0.25rem;
      border: none;
      background: transparent;
      cursor: pointer;
      font-size: 1.25rem;
      line-height: 1;
      opacity: 0.5;
      transition: opacity 0.2s;
      flex-shrink: 0;
    }

    .alert-close:hover {
      opacity: 1;
    }

    .alert-info {
      background: var(--primary-500-alpha-10);
      border-color: var(--primary-500-alpha-30);
      color: var(--primary-800);
    }

    .alert-success {
      background: var(--success-500-alpha-10);
      border-color: var(--success-500-alpha-30);
      color: var(--success-700);
    }

    .alert-warning {
      background: var(--warning-500-alpha-10);
      border-color: var(--warning-500-alpha-30);
      color: var(--warning-700);
    }

    .alert-danger {
      background: var(--danger-500-alpha-10);
      border-color: var(--danger-500-alpha-30);
      color: var(--danger-700);
    }

  `]
})
export class AlertComponent {
  @Input() type: AlertType = 'info';
  @Input() title?: string;
  @Input() dismissible: boolean = false;
  @Output() dismissed = new EventEmitter<void>();

  isDismissed: boolean = false;

  getIcon(): string {
    const icons = {
      info: 'ℹ️',
      success: '✅',
      warning: '⚠️',
      danger: '❌'
    };
    return icons[this.type];
  }

  onDismiss(): void {
    this.isDismissed = true;
    this.dismissed.emit();
  }
}
