import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';

export const guestGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  
  const token = localStorage.getItem('token');
  const user = localStorage.getItem('user');
  
  // Se já estiver logado, redirecionar para o painel apropriado
  if (token && user) {
    const userData = JSON.parse(user);
    const userRole = userData.role as number;
    
    // Admin vai para /admin, outros para /painel
    if (userRole === 3) {
      router.navigate(['/admin']);
    } else {
      router.navigate(['/painel']);
    }
    return false;
  }
  
  // Se não estiver logado, permite acesso às rotas públicas
  return true;
};
