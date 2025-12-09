import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface Tab {
  id: string;
  label: string;
  icon?: string;
  badge?: number;
  disabled?: boolean;
}

@Component({
  selector: 'app-tabs',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="tabs-container">
      <div class="tabs-header" role="tablist">
        <button
          *ngFor="let tab of tabs"
          type="button"
          role="tab"
          class="tab"
          [class.active]="activeTab === tab.id"
          [class.disabled]="tab.disabled"
          [disabled]="tab.disabled"
          [attr.aria-selected]="activeTab === tab.id"
          [attr.aria-controls]="'tabpanel-' + tab.id"
          (click)="selectTab(tab.id)"
        >
          <span *ngIf="tab.icon" class="tab-icon">{{ tab.icon }}</span>
          <span class="tab-label">{{ tab.label }}</span>
          <span *ngIf="tab.badge !== undefined" class="tab-badge">{{ tab.badge }}</span>
        </button>
      </div>
      
      <div class="tabs-content">
        <ng-content></ng-content>
      </div>
    </div>
  `,
  styles: [`
    .tabs-container {
      display: flex;
      flex-direction: column;
    }

    .tabs-header {
      display: flex;
      gap: 0.25rem;
      border-bottom: 2px solid var(--border-primary);
    }

    .tab {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      padding: 0.75rem 1rem;
      background: transparent;
      border: none;
      border-bottom: 2px solid transparent;
      color: var(--text-secondary);
      font-size: 0.875rem;
      font-weight: 500;
      cursor: pointer;
      transition: all 0.2s;
      margin-bottom: -2px;
    }

    .tab:hover:not(.disabled) {
      color: var(--text-primary);
      background: var(--bg-secondary);
    }

    .tab.active {
      color: var(--primary-600);
      border-bottom-color: var(--primary-600);
    }

    .tab.disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

    .tab-icon {
      font-size: 1rem;
    }

    .tab-label {
      white-space: nowrap;
    }

    .tab-badge {
      padding: 0.125rem 0.5rem;
      background: var(--primary-600);
      color: var(--text-inverse);
      border-radius: 1rem;
      font-size: 0.75rem;
      font-weight: 600;
    }

    .tabs-content {
      padding: 1.5rem 0;
    }

    @media (max-width: 640px) {
      .tabs-header {
        overflow-x: auto;
      }

      .tab {
        font-size: 0.8125rem;
        padding: 0.625rem 0.875rem;
      }
    }
  `]
})
export class TabsComponent {
  @Input() tabs: Tab[] = [];
  @Input() activeTab?: string;
  
  @Output() tabChange = new EventEmitter<string>();

  ngOnInit(): void {
    if (!this.activeTab && this.tabs.length > 0) {
      const firstEnabledTab = this.tabs.find(t => !t.disabled);
      if (firstEnabledTab) {
        this.activeTab = firstEnabledTab.id;
      }
    }
  }

  selectTab(tabId: string): void {
    const tab = this.tabs.find(t => t.id === tabId);
    if (tab && !tab.disabled) {
      this.activeTab = tabId;
      this.tabChange.emit(tabId);
    }
  }
}
