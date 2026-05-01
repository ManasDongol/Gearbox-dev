import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { Auth, Role } from '../../services/auth/auth';

export function roleGuard(...roles: Role[]): CanActivateFn {
  return () => {
    const auth = inject(Auth);
    const router = inject(Router);

    const user = auth.user;

   
    if (!user) {
      return router.createUrlTree(['/login']);
    }

    
    const allowed = user.roles.some(r => roles.includes(r as Role));

    return allowed
      ? true
      : router.createUrlTree(['/home']);
  };
}