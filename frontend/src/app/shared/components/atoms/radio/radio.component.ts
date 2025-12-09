import { Component, Input, Output, EventEmitter, forwardRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'app-radio',
  standalone: true,
  imports: [CommonModule],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => RadioComponent),
      multi: true
    }
  ],
  template: `
    <label class="radio-container" [class.radio-disabled]="disabled">
      <input
        type="radio"
        class="radio-input"
        [checked]="isChecked"
        [disabled]="disabled"
        [name]="name"
        [value]="value"
        (change)="onRadioChange($event)"
      />
      <span class="radio-custom">
        <span *ngIf="isChecked" class="radio-dot"></span>
      </span>
      <span *ngIf="label" class="radio-label">
        {{ label }}
      </span>
      <ng-content></ng-content>
    </label>
  `,
  styles: [`
    .radio-container {
      display: inline-flex;
      align-items: center;
      gap: 0.5rem;
      cursor: pointer;
      user-select: none;
    }

    .radio-input {
      position: absolute;
      opacity: 0;
      cursor: pointer;
      height: 0;
      width: 0;
    }

    .radio-custom {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      width: 1.25rem;
      height: 1.25rem;
      border: 2px solid var(--text-secondary);
      border-radius: 50%;
      background: var(--card-bg);
      transition: all 0.2s;
    }

    .radio-input:checked ~ .radio-custom {
      border-color: var(--primary-600);
    }

    .radio-input:focus ~ .radio-custom {
      box-shadow: 0 0 0 3px var(--primary-500-alpha-10);
    }

    .radio-dot {
      width: 0.625rem;
      height: 0.625rem;
      border-radius: 50%;
      background: var(--primary-600);
    }

    .radio-label {
      font-size: 0.875rem;
      color: var(--text-secondary);
    }

    .radio-disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

  `]
})
export class RadioComponent implements ControlValueAccessor {
  @Input() label?: string;
  @Input() value: any;
  @Input() name?: string;
  @Input() disabled: boolean = false;
  @Output() valueChange = new EventEmitter<any>();

  selectedValue: any;
  onChange: any = () => {};
  onTouched: any = () => {};

  get isChecked(): boolean {
    return this.selectedValue === this.value;
  }

  onRadioChange(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.selectedValue = this.value;
    this.onChange(this.value);
    this.onTouched();
    this.valueChange.emit(this.value);
  }

  writeValue(value: any): void {
    this.selectedValue = value;
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
