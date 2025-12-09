import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BadgeComponent } from '../../atoms/badge/badge.component';
import { AvatarComponent } from '../../atoms/avatar/avatar.component';

export interface CardAction {
  label: string;
  icon?: string;
  handler: () => void;
}

@Component({
  selector: 'app-card',
  standalone: true,
  imports: [CommonModule, BadgeComponent, AvatarComponent],
  template: `
    <div class="card" [class]="'card-' + variant">
      <div *ngIf="badge || actions?.length" class="card-header-actions">
        <app-badge *ngIf="badge" [variant]="badgeVariant">{{ badge }}</app-badge>
        <div class="card-actions">
          <button
            *ngFor="let action of actions"
            type="button"
            class="card-action-btn"
            (click)="action.handler()"
            [title]="action.label"
          >
            {{ action.icon || action.label }}
          </button>
        </div>
      </div>

      <div class="card-header" *ngIf="title || subtitle">
        <div class="card-header-content">
          <app-avatar
            *ngIf="avatar"
            [src]="avatar"
            [size]="'md'"
          ></app-avatar>
          <div>
            <h3 *ngIf="title" class="card-title">{{ title }}</h3>
            <p *ngIf="subtitle" class="card-subtitle">{{ subtitle }}</p>
          </div>
        </div>
      </div>

      <div class="card-body">
        <ng-content></ng-content>
      </div>

      <div class="card-footer" *ngIf="hasFooter">
        <ng-content select="[card-footer]"></ng-content>
      </div>
    </div>
  `,
  styles: [`
    .card {
      background: var(--card-bg);
      border: 1px solid var(--border-primary);
      border-radius: 0.5rem;
      overflow: hidden;
      transition: all 0.2s;
    }

    .card-elevated {
      box-shadow: 0 1px 3px 0 var(--black-alpha-10), 0 1px 2px 0 var(--black-alpha-06);
    }

    .card-elevated:hover {
      box-shadow: 0 10px 15px -3px var(--black-alpha-10), 0 4px 6px -2px var(--black-alpha-05);
    }

    .card-interactive {
      cursor: pointer;
    }

    .card-interactive:hover {
      border-color: var(--primary-600);
    }

    .card-header-actions {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 1rem 1rem 0 1rem;
    }

    .card-actions {
      display: flex;
      gap: 0.5rem;
    }

    .card-action-btn {
      padding: 0.25rem;
      background: transparent;
      border: none;
      color: var(--text-secondary);
      cursor: pointer;
      font-size: 1.25rem;
      transition: color 0.2s;
    }

    .card-action-btn:hover {
      color: var(--text-primary);
    }

    .card-header {
      padding: 1rem;
    }

    .card-header-content {
      display: flex;
      align-items: center;
      gap: 0.75rem;
    }

    .card-title {
      font-size: 1.125rem;
      font-weight: 600;
      color: var(--text-primary);
      margin: 0;
    }

    .card-subtitle {
      font-size: 0.875rem;
      color: var(--text-secondary);
      margin: 0.25rem 0 0 0;
    }

    .card-body {
      padding: 1rem;
    }

    .card-footer {
      padding: 1rem;
      border-top: 1px solid var(--border-primary);
      background: var(--bg-secondary);
    }

  `]
})
export class CardComponent {
  @Input() title?: string;
  @Input() subtitle?: string;
  @Input() avatar?: string;
  @Input() badge?: string;
  @Input() badgeVariant: 'primary' | 'secondary' | 'success' | 'danger' | 'warning' | 'info' = 'primary';
  @Input() variant: 'flat' | 'elevated' | 'interactive' = 'flat';
  @Input() actions?: CardAction[];
  @Input() hasFooter: boolean = false;

  @Output() cardClick = new EventEmitter<void>();
}
