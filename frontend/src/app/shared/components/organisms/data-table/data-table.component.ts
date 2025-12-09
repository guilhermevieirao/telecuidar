import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PaginationComponent } from '../../molecules/pagination/pagination.component';
import { EmptyStateComponent } from '../../molecules/empty-state/empty-state.component';
import { SpinnerComponent } from '../../atoms/spinner/spinner.component';

export interface TableColumn {
  key: string;
  label: string;
  sortable?: boolean;
  width?: string;
}

export interface TableAction {
  label: string;
  icon?: string;
  variant?: 'primary' | 'secondary' | 'danger';
  handler: (row: any) => void;
}

@Component({
  selector: 'app-data-table',
  standalone: true,
  imports: [CommonModule, PaginationComponent, EmptyStateComponent, SpinnerComponent],
  template: `
    <div class="data-table-container">
      <app-spinner *ngIf="loading"></app-spinner>
      
      <div *ngIf="!loading && data.length === 0">
        <app-empty-state
          [icon]="emptyIcon"
          [title]="emptyTitle"
          [description]="emptyDescription"
        ></app-empty-state>
      </div>
      
      <div *ngIf="!loading && data.length > 0" class="table-wrapper">
        <table class="data-table">
          <thead>
            <tr>
              <th 
                *ngFor="let column of columns"
                [style.width]="column.width"
                [class.sortable]="column.sortable"
                (click)="column.sortable && onSort(column.key)"
              >
                {{ column.label }}
                <span *ngIf="column.sortable && sortColumn === column.key" class="sort-indicator">
                  {{ sortDirection === 'asc' ? '↑' : '↓' }}
                </span>
              </th>
              <th *ngIf="actions && actions.length > 0" class="actions-column">
                Ações
              </th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let row of data" [class.row-clickable]="rowClickable" (click)="onRowClick(row)">
              <td *ngFor="let column of columns">
                <ng-container *ngIf="!getCellTemplate(column.key); else customCell">
                  {{ getValue(row, column.key) }}
                </ng-container>
                <ng-template #customCell>
                  <ng-content [select]="'[cell-' + column.key + ']'"></ng-content>
                </ng-template>
              </td>
              <td *ngIf="actions && actions.length > 0" class="actions-cell">
                <button
                  *ngFor="let action of actions"
                  type="button"
                  [class]="'action-btn action-btn-' + (action.variant || 'primary')"
                  (click)="action.handler(row); $event.stopPropagation()"
                >
                  <span *ngIf="action.icon">{{ action.icon }}</span>
                  {{ action.label }}
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      
      <app-pagination
        *ngIf="!loading && pagination && data.length > 0"
        [pageInfo]="pagination"
        (pageChange)="onPageChange($event)"
      ></app-pagination>
    </div>
  `,
  styles: [`
    .data-table-container {
      position: relative;
      width: 100%;
    }

    .table-wrapper {
      overflow-x: auto;
      border-radius: 0.5rem;
      border: 1px solid var(--border-primary);
    }

    .data-table {
      width: 100%;
      border-collapse: collapse;
      font-size: 0.875rem;
    }

    .data-table thead {
      background: var(--bg-secondary);
    }

    .data-table th {
      padding: 0.75rem 1rem;
      text-align: left;
      font-weight: 600;
      color: var(--text-secondary);
      border-bottom: 1px solid var(--border-primary);
    }

    .data-table th.sortable {
      cursor: pointer;
      user-select: none;
    }

    .data-table th.sortable:hover {
      background: var(--bg-tertiary);
    }

    .sort-indicator {
      margin-left: 0.25rem;
      font-size: 0.75rem;
    }

    .data-table td {
      padding: 0.75rem 1rem;
      border-bottom: 1px solid var(--border-primary);
      color: var(--text-primary);
    }

    .data-table tbody tr:hover {
      background: var(--bg-secondary);
    }

    .row-clickable {
      cursor: pointer;
    }

    .actions-column,
    .actions-cell {
      text-align: right;
      white-space: nowrap;
    }

    .actions-cell {
      display: flex;
      gap: 0.5rem;
      justify-content: flex-end;
    }

    .action-btn {
      padding: 0.375rem 0.75rem;
      font-size: 0.75rem;
      border: 1px solid;
      border-radius: 0.25rem;
      cursor: pointer;
      transition: all 0.2s;
      background: transparent;
    }

    .action-btn-primary {
      color: var(--primary-600);
      border-color: var(--primary-600);
    }

    .action-btn-primary:hover {
      background: var(--primary-600);
      color: var(--text-inverse);
    }

    .action-btn-secondary {
      color: var(--text-secondary);
      border-color: var(--text-secondary);
    }

    .action-btn-secondary:hover {
      background: var(--text-secondary);
      color: var(--text-inverse);
    }

    .action-btn-danger {
      color: var(--danger-500);
      border-color: var(--danger-500);
    }

    .action-btn-danger:hover {
      background: var(--danger-500);
      color: var(--text-inverse);
    }

  `]
})
export class DataTableComponent {
  @Input() columns: TableColumn[] = [];
  @Input() data: any[] = [];
  @Input() actions?: TableAction[];
  @Input() loading: boolean = false;
  @Input() pagination?: any;
  @Input() rowClickable: boolean = false;
  @Input() emptyIcon: string = '📋';
  @Input() emptyTitle: string = 'Nenhum dado encontrado';
  @Input() emptyDescription?: string;
  
  @Output() rowClick = new EventEmitter<any>();
  @Output() pageChange = new EventEmitter<number>();
  @Output() sort = new EventEmitter<{ column: string; direction: 'asc' | 'desc' }>();

  sortColumn?: string;
  sortDirection: 'asc' | 'desc' = 'asc';

  getValue(row: any, key: string): any {
    return key.split('.').reduce((obj, k) => obj?.[k], row);
  }

  getCellTemplate(key: string): boolean {
    // Placeholder for custom cell templates
    return false;
  }

  onRowClick(row: any): void {
    if (this.rowClickable) {
      this.rowClick.emit(row);
    }
  }

  onPageChange(page: number): void {
    this.pageChange.emit(page);
  }

  onSort(column: string): void {
    if (this.sortColumn === column) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortColumn = column;
      this.sortDirection = 'asc';
    }
    this.sort.emit({ column, direction: this.sortDirection });
  }
}
