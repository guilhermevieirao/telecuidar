import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '@core/services/auth.service';

export const roleGuard = (allowedRoles: string[]): CanActivateFn => {
  return (route, state) => {
    const authService = inject(AuthService);
    const router = inject(Router);

    const user = authService.currentUser();
    
    if (!user) {
      router.navigate(['/auth/login']);
      return false;
    }

    if (!allowedRoles.includes(user.role)) {
      router.navigate(['/painel']);
      return false;
    }

    return true;
  };
};
