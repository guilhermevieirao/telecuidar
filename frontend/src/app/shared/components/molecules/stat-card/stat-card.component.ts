import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BadgeComponent } from '../../atoms/badge/badge.component';

export interface StatCardData {
  label: string;
  value: string | number;
  change?: number;
  changeLabel?: string;
  icon?: string;
  variant?: 'primary' | 'success' | 'warning' | 'danger';
}

@Component({
  selector: 'app-stat-card',
  standalone: true,
  imports: [CommonModule, BadgeComponent],
  template: `
    <div class="stat-card" [class]="'stat-card-' + (data.variant || 'primary')">
      <div class="stat-header">
        <span class="stat-label">{{ data.label }}</span>
        <span *ngIf="data.icon" class="stat-icon">{{ data.icon }}</span>
      </div>
      
      <div class="stat-value">{{ data.value }}</div>
      
      <div *ngIf="data.change !== undefined" class="stat-footer">
        <app-badge 
          [variant]="data.change >= 0 ? 'success' : 'danger'"
          size="sm"
        >
          {{ data.change >= 0 ? '↑' : '↓' }} {{ Math.abs(data.change) }}%
        </app-badge>
        <span *ngIf="data.changeLabel" class="stat-change-label">
          {{ data.changeLabel }}
        </span>
      </div>
    </div>
  `,
  styles: [`
    .stat-card {
      padding: 1.5rem;
      border-radius: 0.75rem;
      background: var(--card-bg);
      border: 1px solid var(--border-primary);
      transition: all 0.2s;
    }

    .stat-card:hover {
      box-shadow: 0 4px 6px -1px var(--black-alpha-10);
    }

    .stat-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 0.75rem;
    }

    .stat-label {
      font-size: 0.875rem;
      color: var(--text-secondary);
      font-weight: 500;
    }

    .stat-icon {
      font-size: 1.5rem;
      opacity: 0.5;
    }

    .stat-value {
      font-size: 2rem;
      font-weight: 700;
      color: var(--text-primary);
      margin-bottom: 0.5rem;
    }

    .stat-footer {
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }

    .stat-change-label {
      font-size: 0.75rem;
      color: var(--text-tertiary);
    }

    .stat-card-primary {
      border-color: var(--primary-500-alpha-20);
    }

    .stat-card-success {
      border-color: var(--success-500-alpha-20);
    }

    .stat-card-warning {
      border-color: var(--warning-500-alpha-20);
    }

    .stat-card-danger {
      border-color: var(--danger-500-alpha-20);
    }

  `]
})
export class StatCardComponent {
  @Input() data!: StatCardData;
  Math = Math;
}
