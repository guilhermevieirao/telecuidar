import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-label',
  standalone: true,
  imports: [CommonModule],
  template: `
    <label 
      [attr.for]="htmlFor"
      class="label"
      [class.label-required]="required"
    >
      <ng-content></ng-content>
      <span *ngIf="required" class="required-indicator">*</span>
    </label>
  `,
  styles: [`
    .label {
      display: inline-block;
      font-weight: 500;
      margin-bottom: 0.5rem;
      color: var(--text-primary, var(--bg-tertiary));
      font-size: 0.875rem;
    }

    .required-indicator {
      color: var(--danger-500);
      margin-left: 0.25rem;
    }

  `]
})
export class LabelComponent {
  @Input() htmlFor?: string;
  @Input() required: boolean = false;
}
