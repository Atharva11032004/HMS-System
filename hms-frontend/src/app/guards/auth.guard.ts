import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth';

export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  if (auth.isLoggedIn()) return true;
  router.navigate(['/sign-in']);
  return false;
};

export const roleGuard = (allowedRole: string): CanActivateFn => () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  if (!auth.isLoggedIn()) { router.navigate(['/sign-in']); return false; }
  const role = auth.getRole()?.toLowerCase();
  if (role === allowedRole.toLowerCase()) return true;
  auth.redirectToDashboard();
  return false;
};
