import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./features/landing/landing.component').then(m => m.LandingComponent)
  },
  {
    path: 'entrar',
    loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'cadastrar',
    loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent)
  },
  {
    path: 'recuperar-senha',
    loadComponent: () => import('./features/auth/forgot-password/forgot-password.component').then(m => m.ForgotPasswordComponent)
  },
  {
    path: 'confirmar-email',
    loadComponent: () => import('./features/auth/confirm-email/confirm-email.component').then(m => m.ConfirmEmailComponent)
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
  {
    path: 'admin',
    redirectTo: 'administracao',
    pathMatch: 'full'
  },
  {
    path: 'audit-logs',
    redirectTo: 'logs-auditoria',
    pathMatch: 'full'
  },
  // Rotas protegidas
  {
    path: 'painel',
    loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent),
    canActivate: [authGuard]
  },
  {
    path: 'perfil',
    loadComponent: () => import('./features/profile/profile.component').then(m => m.ProfileComponent),
    canActivate: [authGuard],
    data: { breadcrumbParent: '/painel' }
  },
  {
    path: 'administracao',
    loadComponent: () => import('./features/admin/admin.component').then(m => m.AdminComponent),
    canActivate: [authGuard],
    data: { roles: [3], breadcrumbParent: '/painel' } // Apenas Administrador
  },
  {
    path: 'logs-auditoria',
    loadComponent: () => import('./features/audit-logs/audit-logs.component').then(m => m.AuditLogsComponent),
    canActivate: [authGuard],
    data: { roles: [3], breadcrumbParent: '/administracao' } // Apenas Administrador
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
