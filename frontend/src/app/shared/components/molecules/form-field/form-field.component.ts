import { Component, Input, Output, EventEmitter, forwardRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { LabelComponent } from '../../atoms/label/label.component';

@Component({
  selector: 'app-form-field',
  standalone: true,
  imports: [CommonModule, LabelComponent],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => FormFieldComponent),
      multi: true
    }
  ],
  template: `
    <div class="form-field">
      <app-label 
        *ngIf="label"
        [htmlFor]="id"
        [required]="required"
      >
        {{ label }}
      </app-label>
      
      <input
        [id]="id"
        [type]="type"
        [placeholder]="placeholder"
        [disabled]="disabled"
        [readonly]="readonly"
        [value]="value"
        [class.input-error]="error"
        class="form-input"
        (input)="onInputChange($event)"
        (blur)="onTouched()"
      />
      
      <span *ngIf="hint && !error" class="hint-message">
        {{ hint }}
      </span>
      
      <span *ngIf="error && errorMessage" class="error-message">
        {{ errorMessage }}
      </span>
    </div>
  `,
  styles: [`
    .form-field {
      display: flex;
      flex-direction: column;
      gap: 0.25rem;
      width: 100%;
    }

    .form-input {
      width: 100%;
      padding: 0.625rem 0.75rem;
      font-size: 0.875rem;
      border: 1px solid var(--text-secondary);
      border-radius: 0.375rem;
      background: var(--card-bg);
      color: var(--text-secondary);
      transition: all 0.2s;
    }

    .form-input::placeholder {
      color: var(--text-tertiary);
    }

    .form-input:hover:not(:disabled):not(:readonly) {
      border-color: var(--text-tertiary);
    }

    .form-input:focus {
      outline: none;
      border-color: var(--primary-600);
      box-shadow: 0 0 0 3px var(--primary-500-alpha-10);
    }

    .form-input:disabled,
    .form-input:readonly {
      background: var(--bg-tertiary);
      cursor: not-allowed;
      opacity: 0.6;
    }

    .input-error {
      border-color: var(--danger-500) !important;
    }

    .input-error:focus {
      box-shadow: 0 0 0 3px var(--danger-500-alpha-10) !important;
    }

    .hint-message,
    .error-message {
      font-size: 0.75rem;
      margin-top: 0.25rem;
    }

    .hint-message {
      color: var(--text-secondary);
    }

    .error-message {
      color: var(--danger-500);
    }

  `]
})
export class FormFieldComponent implements ControlValueAccessor {
  @Input() id?: string;
  @Input() label?: string;
  @Input() type: string = 'text';
  @Input() placeholder?: string;
  @Input() disabled: boolean = false;
  @Input() readonly: boolean = false;
  @Input() required: boolean = false;
  @Input() error: boolean = false;
  @Input() errorMessage?: string;
  @Input() hint?: string;
  @Output() valueChange = new EventEmitter<string>();

  value: string = '';
  onChange: any = () => {};
  onTouched: any = () => {};

  onInputChange(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.value = target.value;
    this.onChange(this.value);
    this.valueChange.emit(this.value);
  }

  writeValue(value: string): void {
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
