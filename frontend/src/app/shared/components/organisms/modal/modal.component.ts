import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-modal',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div *ngIf="isOpen" class="fixed inset-0 z-50 overflow-y-auto" (click)="onBackdropClick($event)">
      <!-- Backdrop -->
      <div class="fixed inset-0 bg-black bg-opacity-50 transition-opacity"></div>
      
      <!-- Modal -->
      <div class="flex items-center justify-center min-h-screen px-4 pt-4 pb-20 text-center sm:p-0">
        <div
          class="relative inline-block align-bottom bg-white rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-lg sm:w-full"
          (click)="$event.stopPropagation()"
        >
          <!-- Header -->
          <div *ngIf="title" class="bg-white px-6 pt-5 pb-4 sm:p-6 sm:pb-4 border-b border-gray-200">
            <div class="flex items-start justify-between">
              <h3 class="text-lg font-semibold text-gray-900">{{ title }}</h3>
              <button
                *ngIf="closable"
                type="button"
                class="rounded-md text-gray-400 hover:text-gray-500 focus:outline-none"
                (click)="close()"
              >
                <span class="sr-only">Fechar</span>
                <svg class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
                </svg>
              </button>
            </div>
          </div>
          
          <!-- Body -->
          <div class="bg-white px-6 py-4">
            <ng-content></ng-content>
          </div>
          
          <!-- Footer -->
          <div *ngIf="hasFooter" class="bg-gray-50 px-6 py-4 sm:flex sm:flex-row-reverse gap-3">
            <ng-content select="[footer]"></ng-content>
          </div>
        </div>
      </div>
    </div>
  `
})
export class ModalComponent {
  @Input() isOpen = false;
  @Input() title = '';
  @Input() closable = true;
  @Input() closeOnBackdrop = true;
  @Input() hasFooter = false;
  
  @Output() closed = new EventEmitter<void>();

  close(): void {
    this.isOpen = false;
    this.closed.emit();
  }

  onBackdropClick(event: Event): void {
    if (this.closeOnBackdrop && event.target === event.currentTarget) {
      this.close();
    }
  }
}
