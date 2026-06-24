import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { map } from 'rxjs';
import { AuthService } from './auth.service';

export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.loadCurrentUser().pipe(
    map(user => user ? true : router.createUrlTree(['/login']))
  );
};

export const guestGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.loadCurrentUser().pipe(
    map(user => user ? router.createUrlTree(['/viewer']) : true)
  );
};

export const roleGuard: CanActivateFn = route => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const roles = (route.data?.['roles'] as string[]) ?? [];

  return authService.loadCurrentUser().pipe(
    map(user => {
      if (!user) return router.createUrlTree(['/login']);
      return roles.length === 0 || roles.some(role => user.roles.includes(role))
        ? true
        : router.createUrlTree(['/home']);
    })
  );
};
