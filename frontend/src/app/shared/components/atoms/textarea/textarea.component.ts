import { Component, Input, Output, EventEmitter, forwardRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'app-textarea',
  standalone: true,
  imports: [CommonModule],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => TextareaComponent),
      multi: true
    }
  ],
  template: `
    <div class="textarea-container">
      <textarea
        class="textarea-input"
        [class.textarea-error]="error"
        [placeholder]="placeholder"
        [disabled]="disabled"
        [rows]="rows"
        [maxLength]="maxLength"
        [value]="value"
        (input)="onInputChange($event)"
        (blur)="onTouched()"
      ></textarea>
      <span *ngIf="showCounter && maxLength" class="character-counter">
        {{ value.length || 0 }} / {{ maxLength }}
      </span>
    </div>
    <span *ngIf="error && errorMessage" class="error-message">
      {{ errorMessage }}
    </span>
  `,
  styles: [`
    .textarea-container {
      position: relative;
      width: 100%;
    }

    .textarea-input {
      width: 100%;
      padding: 0.625rem 0.75rem;
      font-size: 0.875rem;
      line-height: 1.5;
      border: 1px solid var(--text-secondary);
      border-radius: 0.375rem;
      background: var(--card-bg);
      color: var(--text-secondary);
      resize: vertical;
      transition: all 0.2s;
      font-family: inherit;
    }

    .textarea-input::placeholder {
      color: var(--text-tertiary);
    }

    .textarea-input:hover:not(:disabled) {
      border-color: var(--text-tertiary);
    }

    .textarea-input:focus {
      outline: none;
      border-color: var(--primary-600);
      box-shadow: 0 0 0 3px var(--primary-500-alpha-10);
    }

    .textarea-input:disabled {
      background: var(--bg-tertiary);
      cursor: not-allowed;
      opacity: 0.6;
    }

    .textarea-error {
      border-color: var(--danger-500) !important;
    }

    .textarea-error:focus {
      box-shadow: 0 0 0 3px var(--danger-500-alpha-10) !important;
    }

    .character-counter {
      position: absolute;
      bottom: 0.5rem;
      right: 0.75rem;
      font-size: 0.75rem;
      color: var(--text-secondary);
      background: var(--card-bg);
      padding: 0.125rem 0.25rem;
      border-radius: 0.25rem;
    }

    .error-message {
      display: block;
      margin-top: 0.25rem;
      font-size: 0.75rem;
      color: var(--danger-500);
    }

  `]
})
export class TextareaComponent implements ControlValueAccessor {
  @Input() placeholder?: string;
  @Input() disabled: boolean = false;
  @Input() rows: number = 4;
  @Input() maxLength?: number;
  @Input() showCounter: boolean = false;
  @Input() error: boolean = false;
  @Input() errorMessage?: string;
  @Output() valueChange = new EventEmitter<string>();

  value: string = '';
  onChange: any = () => {};
  onTouched: any = () => {};

  onInputChange(event: Event): void {
    const target = event.target as HTMLTextAreaElement;
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
