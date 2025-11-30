
import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { ToastService } from '../services/toast.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const toastService = inject(ToastService);
  return next(req).pipe(
    catchError((error) => {
      console.error('HTTP Error:', error);
      // Se for erro de autenticação (sessão expirada)
      if (error.status === 401) {
        // Toast persistente (sem duração) para sessão expirada
        toastService.error('Sessão expirada. Faça login novamente.', 0);
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        router.navigate(['/entrar']);
      }
      // Outros erros podem ser tratados aqui
      return throwError(() => error);
    })
  );
};
