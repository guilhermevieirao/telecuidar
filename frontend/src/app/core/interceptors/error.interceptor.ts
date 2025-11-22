import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((error) => {
      console.error('HTTP Error:', error);
      
      // TODO: Adicionar tratamento de erros customizado
      // TODO: Mostrar toast/notificação de erro
      
      return throwError(() => error);
    })
  );
};
