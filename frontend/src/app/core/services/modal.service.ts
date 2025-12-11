import { Injectable } from '@angular/core';
import { Subject, Observable } from 'rxjs';

export interface ModalConfig {
  title: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  type?: 'confirm' | 'alert';
  variant?: 'danger' | 'warning' | 'info' | 'success';
}

export interface ModalResult {
  confirmed: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class ModalService {
  private modalSubject = new Subject<ModalConfig>();
  private resultSubject = new Subject<ModalResult>();

  modal$ = this.modalSubject.asObservable();
  result$ = this.resultSubject.asObservable();

  open(config: ModalConfig): Observable<ModalResult> {
    this.modalSubject.next(config);
    return new Observable(observer => {
      const subscription = this.result$.subscribe(result => {
        observer.next(result);
        observer.complete();
      });
      return () => subscription.unsubscribe();
    });
  }

  confirm(config: Omit<ModalConfig, 'type'>): Observable<ModalResult> {
    return this.open({ ...config, type: 'confirm' });
  }

  alert(config: Omit<ModalConfig, 'type'>): Observable<ModalResult> {
    return this.open({ ...config, type: 'alert' });
  }

  close(result: ModalResult): void {
    this.resultSubject.next(result);
  }
}
