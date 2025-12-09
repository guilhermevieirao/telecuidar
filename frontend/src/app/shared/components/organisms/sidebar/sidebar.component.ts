import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { LinkComponent } from '../../atoms/link/link.component';
import { IconComponent } from '../../atoms/icon/icon.component';

export interface SidebarItem {
  label: string;
  icon?: string;
  route?: string;
  children?: SidebarItem[];
  badge?: string;
  active?: boolean;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <aside class="sidebar" [class.sidebar-collapsed]="collapsed">
      <div class="sidebar-header">
        <ng-content select="[sidebar-header]"></ng-content>
        
        <button 
          *ngIf="collapsible"
          type="button"
          class="collapse-btn"
          (click)="toggleCollapse()"
          aria-label="Toggle sidebar"
        >
          {{ collapsed ? '→' : '←' }}
        </button>
      </div>
      
      <nav class="sidebar-nav">
        <ul class="sidebar-menu">
          <li *ngFor="let item of items" class="sidebar-item">
            <a
              *ngIf="item.route"
              [routerLink]="item.route"
              routerLinkActive="active"
              class="sidebar-link"
              [class.has-icon]="item.icon"
            >
              <span *ngIf="item.icon" class="sidebar-icon">{{ item.icon }}</span>
              <span class="sidebar-label">{{ item.label }}</span>
              <span *ngIf="item.badge" class="sidebar-badge">{{ item.badge }}</span>
            </a>
            
            <div *ngIf="!item.route" class="sidebar-group">
              <div class="sidebar-group-title">
                <span *ngIf="item.icon" class="sidebar-icon">{{ item.icon }}</span>
                <span class="sidebar-label">{{ item.label }}</span>
              </div>
              
              <ul *ngIf="item.children" class="sidebar-submenu">
                <li *ngFor="let child of item.children">
                  <a
                    [routerLink]="child.route"
                    routerLinkActive="active"
                    class="sidebar-link sidebar-sublink"
                  >
                    <span *ngIf="child.icon" class="sidebar-icon">{{ child.icon }}</span>
                    <span class="sidebar-label">{{ child.label }}</span>
                    <span *ngIf="child.badge" class="sidebar-badge">{{ child.badge }}</span>
                  </a>
                </li>
              </ul>
            </div>
          </li>
        </ul>
      </nav>
      
      <div class="sidebar-footer">
        <ng-content select="[sidebar-footer]"></ng-content>
      </div>
    </aside>
  `,
  styles: [`
    .sidebar {
      display: flex;
      flex-direction: column;
      width: 16rem;
      height: 100%;
      background: var(--card-bg);
      border-right: 1px solid var(--border-primary);
      transition: width 0.3s;
    }

    .sidebar-collapsed {
      width: 4rem;
    }

    .sidebar-collapsed .sidebar-label,
    .sidebar-collapsed .sidebar-badge {
      display: none;
    }

    .sidebar-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 1rem;
      border-bottom: 1px solid var(--border-primary);
    }

    .collapse-btn {
      padding: 0.5rem;
      border: none;
      background: transparent;
      cursor: pointer;
      font-size: 1.25rem;
      color: var(--text-secondary);
      transition: color 0.2s;
    }

    .collapse-btn:hover {
      color: var(--text-primary);
    }

    .sidebar-nav {
      flex: 1;
      overflow-y: auto;
      padding: 1rem 0;
    }

    .sidebar-menu,
    .sidebar-submenu {
      list-style: none;
      padding: 0;
      margin: 0;
    }

    .sidebar-item {
      margin-bottom: 0.25rem;
    }

    .sidebar-link {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      padding: 0.75rem 1rem;
      color: var(--text-secondary);
      text-decoration: none;
      transition: all 0.2s;
      cursor: pointer;
    }

    .sidebar-link:hover {
      background: var(--bg-tertiary);
      color: var(--text-primary);
    }

    .sidebar-link.active {
      background: var(--bg-accent);
      color: var(--primary-600);
      font-weight: 600;
    }

    .sidebar-sublink {
      padding-left: 3rem;
      font-size: 0.875rem;
    }

    .sidebar-icon {
      font-size: 1.25rem;
      flex-shrink: 0;
    }

    .sidebar-label {
      flex: 1;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }

    .sidebar-badge {
      padding: 0.125rem 0.5rem;
      font-size: 0.75rem;
      background: var(--primary-600);
      color: var(--text-inverse);
      border-radius: 1rem;
      font-weight: 600;
    }

    .sidebar-group-title {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      padding: 0.75rem 1rem;
      color: var(--text-tertiary);
      font-size: 0.75rem;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.05em;
    }

    .sidebar-footer {
      padding: 1rem;
      border-top: 1px solid var(--border-primary);
    }

  `]
})
export class SidebarComponent {
  @Input() items: SidebarItem[] = [];
  @Input() collapsible: boolean = true;
  @Input() defaultCollapsed: boolean = false;

  collapsed: boolean = false;

  ngOnInit(): void {
    this.collapsed = this.defaultCollapsed;
  }

  toggleCollapse(): void {
    this.collapsed = !this.collapsed;
  }
}
