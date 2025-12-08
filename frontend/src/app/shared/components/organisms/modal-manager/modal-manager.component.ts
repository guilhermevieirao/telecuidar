import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ConfirmModalComponent } from '../confirm-modal/confirm-modal.component';
import { ModalService, AlertModalOptions, ConfirmModalOptions } from '../../../../core/services/modal.service';

@Component({
  selector: 'app-modal-manager',
  standalone: true,
  imports: [CommonModule, ConfirmModalComponent],
  template: `
    <!-- Alert Modal -->
    <div *ngIf="showAlertModal" class="modal-overlay" (click)="closeAlert()">
      <div class="modal-content" (click)="$event.stopPropagation()">
        <div class="modal-header" [ngClass]="'modal-' + currentAlert.type">
          <h3>{{ currentAlert.title }}</h3>
          <button class="close-btn" (click)="closeAlert()">✕</button>
        </div>
        <div class="modal-body">
          <p>{{ currentAlert.message }}</p>
        </div>
        <div class="modal-footer">
          <button class="btn btn-primary" (click)="closeAlert()">OK</button>
        </div>
      </div>
    </div>

    <!-- Confirm Modal -->
    <app-confirm-modal 
      *ngIf="showConfirmModal"
      [title]="confirmTitle"
      [message]="currentConfirm.message"
      [confirmText]="confirmText"
      [cancelText]="cancelText"
      [confirmType]="confirmType"
      (confirm)="confirmAction()"
      (cancel)="cancelAction()">
    </app-confirm-modal>
  `,
  styles: [`
    .modal-overlay {
      position: fixed;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      background: rgba(0, 0, 0, 0.5);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 10500;
      animation: fadeIn 0.2s ease-out;
    }

    .modal-content {
      background: white;
      border-radius: 0.75rem;
      box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
      max-width: 500px;
      width: 90%;
      animation: slideUp 0.3s ease-out;
    }

    .modal-header {
      padding: 1.5rem;
      border-bottom: 1px solid #e5e7eb;
      display: flex;
      justify-content: space-between;
      align-items: center;

      &.modal-success {
        background: #f0fdf4;
        border-bottom-color: #dcfce7;
      }

      &.modal-error {
        background: #fef2f2;
        border-bottom-color: #fee2e2;
      }

      &.modal-info {
        background: #f0f9ff;
        border-bottom-color: #e0f2fe;
      }

      &.modal-warning {
        background: #fffbeb;
        border-bottom-color: #fef3c7;
      }

      h3 {
        margin: 0;
        font-weight: 600;
      }
    }

    .modal-body {
      padding: 1.5rem;
      
      p {
        margin: 0;
        color: #374151;
        line-height: 1.6;
      }
    }

    .modal-footer {
      padding: 1rem 1.5rem;
      border-top: 1px solid #e5e7eb;
      display: flex;
      justify-content: flex-end;
      gap: 0.75rem;
    }

    .btn {
      padding: 0.5rem 1rem;
      border-radius: 0.375rem;
      border: none;
      cursor: pointer;
      font-weight: 500;
      transition: all 0.2s;

      &:hover {
        opacity: 0.9;
      }
    }

    .btn-primary {
      background: #3b82f6;
      color: white;

      &:hover {
        background: #2563eb;
      }
    }

    .close-btn {
      background: transparent;
      border: none;
      cursor: pointer;
      font-size: 1.5rem;
      color: #6b7280;

      &:hover {
        color: #374151;
      }
    }

    @keyframes fadeIn {
      from {
        opacity: 0;
      }
      to {
        opacity: 1;
      }
    }

    @keyframes slideUp {
      from {
        transform: translateY(20px);
        opacity: 0;
      }
      to {
        transform: translateY(0);
        opacity: 1;
      }
    }

    :host-context(html[data-theme="dark"]),
    :host-context(html.dark) {
      .modal-content {
        background: #1f2937;
      }

      .modal-header {
        border-bottom-color: #374151;

        &.modal-success {
          background: #064e3b;
          border-bottom-color: #10b981;
        }

        &.modal-error {
          background: #7f1d1d;
          border-bottom-color: #f87171;
        }

        &.modal-info {
          background: #0c4a6e;
          border-bottom-color: #06b6d4;
        }

        &.modal-warning {
          background: #78350f;
          border-bottom-color: #fbbf24;
        }

        h3 {
          color: #f9fafb;
        }
      }

      .modal-body {
        p {
          color: #d1d5db;
        }
      }

      .modal-footer {
        border-top-color: #374151;
      }

      .close-btn {
        color: #9ca3af;

        &:hover {
          color: #f9fafb;
        }
      }
    }
  `]
})
export class ModalManagerComponent implements OnInit {
  showAlertModal = false;
  showConfirmModal = false;
  currentAlert: AlertModalOptions = { message: '' };
  currentConfirm: ConfirmModalOptions = { message: '' };

  // Getters para valores padrão
  get confirmTitle(): string {
    return this.currentConfirm.title || 'Confirmar ação';
  }

  get confirmText(): string {
    return this.currentConfirm.confirmText || 'Confirmar';
  }

  get cancelText(): string {
    return this.currentConfirm.cancelText || 'Cancelar';
  }

  get confirmType(): 'primary' | 'danger' {
    return (this.currentConfirm.type as 'primary' | 'danger') || 'primary';
  }

  constructor(private modalService: ModalService) {}

  ngOnInit() {
    this.modalService.alert$.subscribe((alert) => {
      this.currentAlert = alert;
      this.showAlertModal = true;
    });

    this.modalService.confirm$.subscribe((confirm) => {
      this.currentConfirm = confirm;
      this.showConfirmModal = true;
    });
  }

  closeAlert() {
    this.showAlertModal = false;
  }

  confirmAction() {
    this.showConfirmModal = false;
    this.modalService.resolveConfirm(true);
  }

  cancelAction() {
    this.showConfirmModal = false;
    this.modalService.resolveConfirm(false);
  }
}
