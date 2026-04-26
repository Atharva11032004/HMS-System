import { Routes } from '@angular/router';
import { Landing } from './pages/landing/landing';
import { SignIn } from './pages/sign-in/sign-in';
import { SignUp } from './pages/sign-up/sign-up';
import { authGuard, roleGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', component: Landing },
  { path: 'sign-in', component: SignIn },
  { path: 'sign-up', component: SignUp },
  {
    path: 'dashboard/owner',
    loadComponent: () => import('./pages/dashboard/owner/owner-dashboard').then(m => m.OwnerDashboard),
    canActivate: [roleGuard('owner')]
  },
  {
    path: 'dashboard/manager',
    loadComponent: () => import('./pages/dashboard/manager/manager-dashboard').then(m => m.ManagerDashboard),
    canActivate: [roleGuard('manager')]
  },
  {
    path: 'dashboard/receptionist',
    loadComponent: () => import('./pages/dashboard/receptionist/receptionist-dashboard').then(m => m.ReceptionistDashboard),
    canActivate: [roleGuard('receptionist')]
  },
  { path: 'dashboard', canActivate: [authGuard], redirectTo: '' },
  { path: '**', redirectTo: '' }
];
