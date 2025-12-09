import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup } from '@angular/forms';
import { FormFieldComponent } from '../../molecules/form-field/form-field.component';
import { ButtonComponent } from '../../atoms/button/button.component';
import { SpinnerComponent } from '../../atoms/spinner/spinner.component';

@Component({
  selector: 'app-form-container',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ButtonComponent,
    SpinnerComponent
  ],
  template: `
    <form
      [formGroup]="formGroup"
      (ngSubmit)="onSubmit()"
      class="form-container"
      [class.form-loading]="loading"
    >
      <div *ngIf="title" class="form-header">
        <h2 class="form-title">{{ title }}</h2>
        <p *ngIf="description" class="form-description">{{ description }}</p>
      </div>

      <div class="form-body">
        <ng-content></ng-content>
      </div>

      <div *ngIf="error" class="form-error">
        {{ error }}
      </div>

      <div class="form-footer">
        <app-button
          *ngIf="showCancel"
          type="button"
          [variant]="'secondary'"
          [disabled]="loading"
          (click)="onCancel()"
        >
          {{ cancelText }}
        </app-button>

        <app-button
          type="submit"
          [variant]="'primary'"
          [disabled]="!formGroup.valid || loading"
          [loading]="loading"
        >
          {{ submitText }}
        </app-button>
      </div>

      <div *ngIf="loading" class="form-loading-overlay">
        <app-spinner></app-spinner>
      </div>
    </form>
  `,
  styles: [`
    .form-container {
      position: relative;
      display: flex;
      flex-direction: column;
      gap: 1.5rem;
    }

    .form-loading {
      pointer-events: none;
      opacity: 0.6;
    }

    .form-header {
      margin-bottom: 1rem;
    }

    .form-title {
      font-size: 1.5rem;
      font-weight: 700;
      color: var(--text-primary);
      margin: 0 0 0.5rem 0;
    }

    .form-description {
      color: var(--text-secondary);
      font-size: 0.875rem;
      margin: 0;
    }

    .form-body {
      display: flex;
      flex-direction: column;
      gap: 1rem;
    }

    .form-error {
      padding: 1rem;
      background: var(--danger-100);
      border: 1px solid var(--danger-200);
      border-radius: 0.5rem;
      color: var(--danger-600);
      font-size: 0.875rem;
    }

    .form-footer {
      display: flex;
      gap: 0.75rem;
      justify-content: flex-end;
      padding-top: 1rem;
      border-top: 1px solid var(--border-primary);
    }

    .form-loading-overlay {
      position: absolute;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      display: flex;
      align-items: center;
      justify-content: center;
      background: var(--white-alpha-80);
      border-radius: 0.5rem;
    }

  `]
})
export class FormContainerComponent {
  @Input() formGroup!: FormGroup;
  @Input() title?: string;
  @Input() description?: string;
  @Input() submitText: string = 'Salvar';
  @Input() cancelText: string = 'Cancelar';
  @Input() showCancel: boolean = true;
  @Input() loading: boolean = false;
  @Input() error?: string;

  @Output() formSubmit = new EventEmitter<any>();
  @Output() formCancel = new EventEmitter<void>();

  onSubmit(): void {
    if (this.formGroup.valid && !this.loading) {
      this.formSubmit.emit(this.formGroup.value);
    }
  }

  onCancel(): void {
    this.formCancel.emit();
  }
}
