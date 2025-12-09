import { Component, Input, Output, EventEmitter, forwardRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

export interface SelectOption {
  label: string;
  value: any;
  disabled?: boolean;
}

@Component({
  selector: 'app-select',
  standalone: true,
  imports: [CommonModule],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => SelectComponent),
      multi: true
    }
  ],
  template: `
    <div class="select-container">
      <select
        class="select-input"
        [class.select-error]="error"
        [disabled]="disabled"
        [value]="value"
        (change)="onSelectChange($event)"
        (blur)="onTouched()"
      >
        <option *ngIf="placeholder" value="" disabled selected>
          {{ placeholder }}
        </option>
        <option 
          *ngFor="let option of options" 
          [value]="option.value"
          [disabled]="option.disabled"
        >
          {{ option.label }}
        </option>
      </select>
      <span class="select-icon">▼</span>
    </div>
    <span *ngIf="error && errorMessage" class="error-message">
      {{ errorMessage }}
    </span>
  `,
  styles: [`
    .select-container {
      position: relative;
      display: inline-block;
      width: 100%;
    }

    .select-input {
      width: 100%;
      padding: 0.625rem 2.5rem 0.625rem 0.75rem;
      font-size: 0.875rem;
      border: 1px solid var(--text-secondary);
      border-radius: 0.375rem;
      background: var(--card-bg);
      color: var(--text-secondary);
      cursor: pointer;
      appearance: none;
      transition: all 0.2s;
    }

    .select-input:hover:not(:disabled) {
      border-color: var(--text-tertiary);
    }

    .select-input:focus {
      outline: none;
      border-color: var(--primary-600);
      box-shadow: 0 0 0 3px var(--primary-500-alpha-10);
    }

    .select-input:disabled {
      background: var(--bg-tertiary);
      cursor: not-allowed;
      opacity: 0.6;
    }

    .select-error {
      border-color: var(--danger-500) !important;
    }

    .select-error:focus {
      box-shadow: 0 0 0 3px var(--danger-500-alpha-10) !important;
    }

    .select-icon {
      position: absolute;
      right: 0.75rem;
      top: 50%;
      transform: translateY(-50%);
      pointer-events: none;
      font-size: 0.625rem;
      color: var(--text-secondary);
    }

    .error-message {
      display: block;
      margin-top: 0.25rem;
      font-size: 0.75rem;
      color: var(--danger-500);
    }

  `]
})
export class SelectComponent implements ControlValueAccessor {
  @Input() options: SelectOption[] = [];
  @Input() placeholder?: string;
  @Input() disabled: boolean = false;
  @Input() error: boolean = false;
  @Input() errorMessage?: string;
  @Output() valueChange = new EventEmitter<any>();

  value: any = '';
  onChange: any = () => {};
  onTouched: any = () => {};

  onSelectChange(event: Event): void {
    const target = event.target as HTMLSelectElement;
    this.value = target.value;
    this.onChange(this.value);
    this.valueChange.emit(this.value);
  }

  writeValue(value: any): void {
    this.value = value || '';
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }
}
