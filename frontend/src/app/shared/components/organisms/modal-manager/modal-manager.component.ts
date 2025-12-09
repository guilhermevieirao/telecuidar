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
          <button class="close-btn" (click)="closeAlert()">?</button>
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
      background: var(--overlay);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 10500;
      animation: fadeIn 0.2s ease-out;
    }

    .modal-content {
      background: var(--card-bg);
      border-radius: 0.75rem;
      box-shadow: 0 20px 60px var(--black-alpha-30);
      max-width: 500px;
      width: 90%;
      animation: slideUp 0.3s ease-out;
    }

    .modal-header {
      padding: 1.5rem;
      border-bottom: 1px solid var(--border-primary);
      display: flex;
      justify-content: space-between;
      align-items: center;

      &.modal-success {
        background: var(--success-50);
        border-bottom-color: var(--success-100);
      }

      &.modal-error {
        background: var(--danger-50);
        border-bottom-color: var(--danger-100);
      }

      &.modal-info {
        background: var(--info-50);
        border-bottom-color: var(--info-100);
      }

      &.modal-warning {
        background: var(--warning-50);
        border-bottom-color: var(--warning-100);
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
        color: var(--text-secondary);
        line-height: 1.6;
      }
    }

    .modal-footer {
      padding: 1rem 1.5rem;
      border-top: 1px solid var(--border-primary);
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
      background: var(--primary-600);
      color: var(--text-inverse);

      &:hover {
        background: var(--primary-700);
      }
    }

    .close-btn {
      background: transparent;
      border: none;
      cursor: pointer;
      font-size: 1.5rem;
      color: var(--text-secondary);

      &:hover {
        color: var(--text-secondary);
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

    .modal-header {
      border-bottom-color: var(--text-secondary);

      &.modal-success {
        background: var(--success-900);
        border-bottom-color: var(--success-500);
      }

      &.modal-error {
        background: var(--danger-900);
        border-bottom-color: var(--danger-400);
      }

      &.modal-info {
        background: var(--info-900);
        border-bottom-color: var(--info-500);
      }

      &.modal-warning {
        background: var(--warning-900);
        border-bottom-color: var(--warning-400);
      }

      h3 {
        color: var(--bg-secondary);
      }
    }

    .modal-body {
      p {
        color: var(--text-secondary);
      }
    }

    .modal-footer {
      border-top-color: var(--text-secondary);
    }

    .close-btn {
      color: var(--text-tertiary);

      &:hover {
        color: var(--bg-secondary);
      }
    }
  `]
})
export class ModalManagerComponent implements OnInit {
  showAlertModal = false;
  showConfirmModal = false;
  currentAlert: AlertModalOptions = { message: '' };
  currentConfirm: ConfirmModalOptions = { message: '' };

  // Getters para valores padr�o
  get confirmTitle(): string {
    return this.currentConfirm.title || 'Confirmar a��o';
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
