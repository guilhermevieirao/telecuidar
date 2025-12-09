import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AvatarComponent } from '../../atoms/avatar/avatar.component';

@Component({
  selector: 'app-user-card',
  standalone: true,
  imports: [CommonModule, AvatarComponent],
  template: `
    <div class="user-card" [class.user-card-horizontal]="!vertical">
      <app-avatar
        [src]="avatarSrc"
        [initials]="avatarInitials"
        [size]="avatarSize"
      ></app-avatar>
      
      <div class="user-info">
        <div class="user-name">{{ name }}</div>
        <div *ngIf="subtitle" class="user-subtitle">{{ subtitle }}</div>
        <div *ngIf="description" class="user-description">{{ description }}</div>
      </div>
    </div>
  `,
  styles: [`
    .user-card {
      display: flex;
      align-items: center;
      gap: 0.75rem;
    }

    .user-card-horizontal {
      flex-direction: row;
    }

    .user-info {
      display: flex;
      flex-direction: column;
      gap: 0.125rem;
      min-width: 0;
    }

    .user-name {
      font-weight: 600;
      font-size: 0.875rem;
      color: var(--text-primary);
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
    }

    .user-subtitle {
      font-size: 0.75rem;
      color: var(--text-secondary);
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
    }

    .user-description {
      font-size: 0.75rem;
      color: var(--text-tertiary);
      margin-top: 0.25rem;
    }

  `]
})
export class UserCardComponent {
  @Input() name: string = '';
  @Input() subtitle?: string;
  @Input() description?: string;
  @Input() avatarSrc?: string;
  @Input() avatarInitials?: string;
  @Input() avatarSize: 'xs' | 'sm' | 'md' | 'lg' | 'xl' = 'md';
  @Input() vertical: boolean = false;
}
