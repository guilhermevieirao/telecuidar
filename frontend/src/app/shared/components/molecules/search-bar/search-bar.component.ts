import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IconComponent } from '../../atoms/icon/icon.component';

@Component({
  selector: 'app-search-bar',
  standalone: true,
  imports: [CommonModule, FormsModule, IconComponent],
  template: `
    <div class="search-bar" [class.search-bar-focused]="isFocused">
      <app-icon icon="🔍" size="sm" class="search-icon"></app-icon>
      
      <input
        type="text"
        class="search-input"
        [placeholder]="placeholder"
        [(ngModel)]="searchValue"
        (ngModelChange)="onSearch($event)"
        (focus)="isFocused = true"
        (blur)="isFocused = false"
      />
      
      <button
        *ngIf="searchValue"
        type="button"
        class="clear-button"
        (click)="clearSearch()"
        aria-label="Limpar busca"
      >
        ✕
      </button>
    </div>
  `,
  styles: [`
    .search-bar {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      padding: 0.625rem 0.75rem;
      border: 1px solid var(--text-secondary);
      border-radius: 0.5rem;
      background: var(--card-bg);
      transition: all 0.2s;
    }

    .search-bar:hover {
      border-color: var(--text-tertiary);
    }

    .search-bar-focused {
      border-color: var(--primary-600);
      box-shadow: 0 0 0 3px var(--primary-500-alpha-10);
    }

    .search-icon {
      color: var(--text-tertiary);
      flex-shrink: 0;
    }

    .search-input {
      flex: 1;
      border: none;
      outline: none;
      background: transparent;
      font-size: 0.875rem;
      color: var(--text-secondary);
    }

    .search-input::placeholder {
      color: var(--text-tertiary);
    }

    .clear-button {
      padding: 0.25rem;
      border: none;
      background: transparent;
      color: var(--text-tertiary);
      cursor: pointer;
      font-size: 0.875rem;
      transition: color 0.2s;
      flex-shrink: 0;
    }

    .clear-button:hover {
      color: var(--text-secondary);
    }

  `]
})
export class SearchBarComponent {
  @Input() placeholder: string = 'Buscar...';
  @Output() search = new EventEmitter<string>();

  searchValue: string = '';
  isFocused: boolean = false;

  onSearch(value: string): void {
    this.search.emit(value);
  }

  clearSearch(): void {
    this.searchValue = '';
    this.search.emit('');
  }
}
