import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-card',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div [class]="cardClasses">
      <div *ngIf="title || subtitle" class="px-6 py-4 border-b border-gray-200">
        <h3 *ngIf="title" class="text-lg font-semibold text-gray-900">{{ title }}</h3>
        <p *ngIf="subtitle" class="mt-1 text-sm text-gray-500">{{ subtitle }}</p>
      </div>
      
      <div [class]="bodyClasses">
        <ng-content></ng-content>
      </div>
      
      <div *ngIf="hasFooter" class="px-6 py-4 bg-gray-50 border-t border-gray-200 rounded-b-lg">
        <ng-content select="[footer]"></ng-content>
      </div>
    </div>
  `
})
export class CardComponent {
  @Input() title = '';
  @Input() subtitle = '';
  @Input() padding = true;
  @Input() hoverable = false;
  @Input() hasFooter = false;

  get cardClasses(): string {
    const base = 'bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden';
    const hover = this.hoverable ? 'hover:shadow-md transition-shadow duration-200' : '';
    return `${base} ${hover}`;
  }

  get bodyClasses(): string {
    return this.padding ? 'px-6 py-4' : '';
  }
}
