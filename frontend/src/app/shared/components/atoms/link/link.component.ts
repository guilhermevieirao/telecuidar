import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

export type LinkVariant = 'primary' | 'secondary' | 'danger';

@Component({
  selector: 'app-link',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <a 
      *ngIf="!routerLink"
      [href]="href"
      [target]="external ? '_blank' : '_self'"
      [rel]="external ? 'noopener noreferrer' : ''"
      class="link"
      [class]="'link-' + variant"
      [class.link-underline]="underline"
    >
      <ng-content></ng-content>
      <span *ngIf="external" class="external-icon">↗</span>
    </a>
    
    <a 
      *ngIf="routerLink"
      [routerLink]="routerLink"
      class="link"
      [class]="'link-' + variant"
      [class.link-underline]="underline"
    >
      <ng-content></ng-content>
    </a>
  `,
  styles: [`
    .link {
      display: inline-flex;
      align-items: center;
      gap: 0.25rem;
      text-decoration: none;
      cursor: pointer;
      transition: all 0.2s;
    }

    .link-primary {
      color: var(--primary-600);
    }

    .link-primary:hover {
      color: var(--primary-700);
    }

    .link-secondary {
      color: var(--text-secondary);
    }

    .link-secondary:hover {
      color: var(--text-secondary);
    }

    .link-danger {
      color: var(--danger-500);
    }

    .link-danger:hover {
      color: var(--danger-600);
    }

    .link-underline {
      text-decoration: underline;
    }

    .external-icon {
      font-size: 0.875em;
    }
  `]
})
export class LinkComponent {
  @Input() href?: string;
  @Input() routerLink?: string | string[];
  @Input() variant: LinkVariant = 'primary';
  @Input() external: boolean = false;
  @Input() underline: boolean = false;
}
