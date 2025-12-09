import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

export type AvatarSize = 'xs' | 'sm' | 'md' | 'lg' | 'xl';

@Component({
  selector: 'app-avatar',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div 
      class="avatar" 
      [class]="'avatar-' + size"
      [style.background-color]="backgroundColor"
    >
      <img 
        *ngIf="src" 
        [src]="src" 
        [alt]="alt"
        class="avatar-image"
      />
      <span 
        *ngIf="!src && initials" 
        class="avatar-initials"
      >
        {{ initials }}
      </span>
      <span 
        *ngIf="!src && !initials" 
        class="avatar-icon"
      >
        👤
      </span>
    </div>
  `,
  styles: [`
    .avatar {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      border-radius: 50%;
      overflow: hidden;
      background-color: var(--border-primary);
      color: var(--text-secondary);
      font-weight: 600;
      flex-shrink: 0;
    }

    .avatar-xs {
      width: 1.5rem;
      height: 1.5rem;
      font-size: 0.625rem;
    }

    .avatar-sm {
      width: 2rem;
      height: 2rem;
      font-size: 0.75rem;
    }

    .avatar-md {
      width: 2.5rem;
      height: 2.5rem;
      font-size: 0.875rem;
    }

    .avatar-lg {
      width: 3rem;
      height: 3rem;
      font-size: 1rem;
    }

    .avatar-xl {
      width: 4rem;
      height: 4rem;
      font-size: 1.25rem;
    }

    .avatar-image {
      width: 100%;
      height: 100%;
      object-fit: cover;
    }

    .avatar-initials,
    .avatar-icon {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 100%;
      height: 100%;
    }
  `]
})
export class AvatarComponent {
  @Input() src?: string;
  @Input() alt: string = 'Avatar';
  @Input() initials?: string;
  @Input() size: AvatarSize = 'md';
  @Input() backgroundColor?: string;
}
