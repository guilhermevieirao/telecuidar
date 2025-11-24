import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { guestGuard } from './core/guards/guest.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./features/landing/landing.component').then(m => m.LandingComponent)
  },
  {
    path: 'entrar',
    loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent),
    canActivate: [guestGuard]
  },
  {
    path: 'cadastrar',
    loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent),
    canActivate: [guestGuard]
  },
  {
    path: 'recuperar-senha',
    loadComponent: () => import('./features/auth/forgot-password/forgot-password.component').then(m => m.ForgotPasswordComponent),
    canActivate: [guestGuard]
  },
  {
    path: 'confirmar-email',
    loadComponent: () => import('./features/auth/confirm-email/confirm-email.component').then(m => m.ConfirmEmailComponent),
    canActivate: [guestGuard]
  },
  // Redirects das rotas antigas para as novas
  {
    path: 'login',
    redirectTo: 'entrar',
    pathMatch: 'full'
  },
  {
    path: 'register',
    redirectTo: 'cadastrar',
    pathMatch: 'full'
  },
  {
    path: 'forgot-password',
    redirectTo: 'recuperar-senha',
    pathMatch: 'full'
  },
  {
    path: 'confirm-email',
    redirectTo: 'confirmar-email',
    pathMatch: 'full'
  },
  {
    path: 'dashboard',
    redirectTo: 'painel',
    pathMatch: 'full'
  },
  {
    path: 'profile',
    redirectTo: 'perfil',
    pathMatch: 'full'
  },
  // Rotas protegidas
  {
    path: 'painel',
    loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent),
    canActivate: [authGuard]
  },
  {
    path: 'admin',
    loadComponent: () => import('./features/admin/admin.component').then(m => m.AdminComponent),
    canActivate: [authGuard],
    data: { roles: [3] }
  },
  {
    path: 'perfil',
    loadComponent: () => import('./features/profile/profile.component').then(m => m.ProfileComponent),
    canActivate: [authGuard],
    data: { breadcrumbParent: '/painel' }
  },
  {
    path: 'teste',
    loadComponent: () => import('./features/video-call/video-call-simple.component').then(m => m.VideoCallSimpleComponent)
  },
  {
    path: '**',
    redirectTo: ''
  }
];
