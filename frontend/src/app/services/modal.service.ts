import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

export interface AlertModalOptions {
  title?: string;
  message: string;
  type?: 'success' | 'error' | 'info' | 'warning';
}

export interface ConfirmModalOptions {
  title?: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  type?: 'primary' | 'danger';
}

@Injectable({
  providedIn: 'root'
})
export class ModalService {
  private alertSubject = new Subject<AlertModalOptions>();
  private confirmSubject = new Subject<ConfirmModalOptions>();
  private confirmResultSubject = new Subject<boolean>();

  alert$ = this.alertSubject.asObservable();
  confirm$ = this.confirmSubject.asObservable();
  confirmResult$ = this.confirmResultSubject.asObservable();

  showAlert(options: AlertModalOptions): void {
    this.alertSubject.next({
      title: options.title || 'Notificação',
      message: options.message,
      type: options.type || 'info'
    });
  }

  showConfirm(options: ConfirmModalOptions): Promise<boolean> {
    this.confirmSubject.next({
      title: options.title || 'Confirmar ação',
      message: options.message,
      confirmText: options.confirmText || 'Confirmar',
      cancelText: options.cancelText || 'Cancelar',
      type: options.type || 'primary'
    });

    return new Promise((resolve) => {
      const subscription = this.confirmResultSubject.subscribe((result) => {
        resolve(result);
        subscription.unsubscribe();
      });
    });
  }

  resolveConfirm(result: boolean): void {
    this.confirmResultSubject.next(result);
  }

  showSuccess(message: string, title?: string): void {
    this.showAlert({
      title: title || 'Sucesso',
      message,
      type: 'success'
    });
  }

  showError(message: string, title?: string): void {
    this.showAlert({
      title: title || 'Erro',
      message,
      type: 'error'
    });
  }

  showInfo(message: string, title?: string): void {
    this.showAlert({
      title: title || 'Informação',
      message,
      type: 'info'
    });
  }
}
