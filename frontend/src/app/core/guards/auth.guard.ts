import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';

export const authGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  
  const token = localStorage.getItem('token');
  const user = localStorage.getItem('user');
  
  if (!token || !user) {
    router.navigate(['/entrar'], { queryParams: { returnUrl: state.url } });
    return false;
  }
  
  // Verificar role se especificado na rota
  const requiredRoles = route.data['roles'] as number[] | undefined;
  if (requiredRoles) {
    const userData = JSON.parse(user);
    const userRole = userData.role as number;
    
    if (!requiredRoles.includes(userRole)) {
      router.navigate(['/painel']);
      return false;
    }
  }
  
  return true;
};
