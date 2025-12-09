import { Component, Input, Output, EventEmitter, forwardRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'app-checkbox',
  standalone: true,
  imports: [CommonModule],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CheckboxComponent),
      multi: true
    }
  ],
  template: `
    <label class="checkbox-container" [class.checkbox-disabled]="disabled">
      <input
        type="checkbox"
        class="checkbox-input"
        [checked]="checked"
        [disabled]="disabled"
        [indeterminate]="indeterminate"
        (change)="onCheckboxChange($event)"
      />
      <span class="checkbox-custom">
        <span *ngIf="checked && !indeterminate" class="checkbox-icon">✓</span>
        <span *ngIf="indeterminate" class="checkbox-icon">-</span>
      </span>
      <span *ngIf="label" class="checkbox-label">
        {{ label }}
      </span>
      <ng-content></ng-content>
    </label>
  `,
  styles: [`
    .checkbox-container {
      display: inline-flex;
      align-items: center;
      gap: 0.5rem;
      cursor: pointer;
      user-select: none;
    }

    .checkbox-input {
      position: absolute;
      opacity: 0;
      cursor: pointer;
      height: 0;
      width: 0;
    }

    .checkbox-custom {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      width: 1.25rem;
      height: 1.25rem;
      border: 2px solid var(--text-secondary);
      border-radius: 0.25rem;
      background: var(--card-bg);
      transition: all 0.2s;
    }

    .checkbox-input:checked ~ .checkbox-custom {
      background: var(--primary-600);
      border-color: var(--primary-600);
    }

    .checkbox-input:indeterminate ~ .checkbox-custom {
      background: var(--primary-600);
      border-color: var(--primary-600);
    }

    .checkbox-input:focus ~ .checkbox-custom {
      box-shadow: 0 0 0 3px var(--primary-500-alpha-10);
    }

    .checkbox-icon {
      color: var(--text-inverse);
      font-size: 0.875rem;
      font-weight: bold;
      line-height: 1;
    }

    .checkbox-label {
      font-size: 0.875rem;
      color: var(--text-secondary);
    }

    .checkbox-disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

  `]
})
export class CheckboxComponent implements ControlValueAccessor {
  @Input() label?: string;
  @Input() disabled: boolean = false;
  @Input() indeterminate: boolean = false;
  @Output() checkedChange = new EventEmitter<boolean>();

  checked: boolean = false;
  onChange: any = () => {};
  onTouched: any = () => {};

  onCheckboxChange(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.checked = target.checked;
    this.onChange(this.checked);
    this.onTouched();
    this.checkedChange.emit(this.checked);
  }

  writeValue(value: boolean): void {
    this.checked = value;
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
