import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EmptyStateComponent } from '../../molecules/empty-state/empty-state.component';
import { SpinnerComponent } from '../../atoms/spinner/spinner.component';

export interface ListItem {
  id: string | number;
  [key: string]: any;
}

@Component({
  selector: 'app-data-list',
  standalone: true,
  imports: [CommonModule, EmptyStateComponent, SpinnerComponent],
  template: `
    <div class="data-list">
      <app-spinner *ngIf="loading"></app-spinner>

      <div *ngIf="!loading && items.length === 0">
        <app-empty-state
          [icon]="emptyIcon"
          [title]="emptyTitle"
          [description]="emptyDescription"
        ></app-empty-state>
      </div>

      <div *ngIf="!loading && items.length > 0" class="list-items">
        <div
          *ngFor="let item of items"
          class="list-item"
          [class.list-item-clickable]="clickable"
          (click)="onItemClick(item)"
        >
          <ng-content [ngTemplateOutlet]="itemTemplate" [ngTemplateOutletContext]="{$implicit: item}"></ng-content>
        </div>
      </div>

      <div *ngIf="hasMore && !loading" class="list-footer">
        <button type="button" class="load-more-btn" (click)="onLoadMore()">
          {{ loadMoreText }}
        </button>
      </div>
    </div>
  `,
  styles: [`
    .data-list {
      width: 100%;
    }

    .list-items {
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
    }

    .list-item {
      padding: 1rem;
      background: var(--card-bg);
      border: 1px solid var(--border-primary);
      border-radius: 0.5rem;
      transition: all 0.2s;
    }

    .list-item-clickable {
      cursor: pointer;
    }

    .list-item-clickable:hover {
      border-color: var(--primary-600);
      box-shadow: 0 1px 3px 0 var(--black-alpha-10);
    }

    .list-footer {
      display: flex;
      justify-content: center;
      padding: 1rem;
    }

    .load-more-btn {
      padding: 0.625rem 1.25rem;
      background: var(--card-bg);
      border: 1px solid var(--border-primary);
      border-radius: 0.5rem;
      color: var(--text-secondary);
      font-weight: 500;
      cursor: pointer;
      transition: all 0.2s;
    }

    .load-more-btn:hover {
      background: var(--bg-tertiary);
      border-color: var(--text-secondary);
    }

  `]
})
export class DataListComponent {
  @Input() items: ListItem[] = [];
  @Input() itemTemplate: any;
  @Input() loading: boolean = false;
  @Input() clickable: boolean = false;
  @Input() hasMore: boolean = false;
  @Input() loadMoreText: string = 'Carregar mais';
  @Input() emptyIcon: string = '📋';
  @Input() emptyTitle: string = 'Nenhum item encontrado';
  @Input() emptyDescription?: string;

  @Output() itemClick = new EventEmitter<ListItem>();
  @Output() loadMore = new EventEmitter<void>();

  onItemClick(item: ListItem): void {
    if (this.clickable) {
      this.itemClick.emit(item);
    }
  }

  onLoadMore(): void {
    this.loadMore.emit();
  }
}
