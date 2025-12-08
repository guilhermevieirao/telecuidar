import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-spinner',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="spinner-overlay" *ngIf="show">
      <div class="spinner-container">
        <div class="spinner" [class.spinner-sm]="size === 'sm'" [class.spinner-lg]="size === 'lg'"></div>
        <p *ngIf="message" class="spinner-message">{{ message }}</p>
      </div>
    </div>
  `,
  styleUrls: ['./spinner.component.scss']
})
export class SpinnerComponent {
  @Input() show: boolean = true;
  @Input() message?: string;
  @Input() size: 'sm' | 'md' | 'lg' = 'md';
}
