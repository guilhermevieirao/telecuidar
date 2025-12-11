import { Routes } from '@angular/router';
import { LandingComponent } from './pages/landing/landing';
import { LoginComponent } from './pages/auth/login/login';
import { RegisterComponent } from './pages/auth/register/register';
import { ForgotPasswordComponent } from './pages/auth/forgot-password/forgot-password';
import { ResetPasswordComponent } from './pages/auth/reset-password/reset-password';
import { VerifyEmailComponent } from './pages/auth/verify-email/verify-email';
import { AdminLayoutComponent } from './pages/admin/admin-layout/admin-layout';
import { DashboardComponent } from './pages/admin/dashboard/dashboard';
import { NotificationsComponent } from './pages/admin/notifications/notifications';
import { UsersComponent } from './pages/admin/users/users';

export const routes: Routes = [
  {
    path: '',
    component: LandingComponent
  },
  {
    path: 'auth',
    children: [
      {
        path: 'login',
        component: LoginComponent
      },
      {
        path: 'register',
        component: RegisterComponent
      },
      {
        path: 'forgot-password',
        component: ForgotPasswordComponent
      },
      {
        path: 'reset-password',
        component: ResetPasswordComponent
      },
      {
        path: 'verify-email',
        component: VerifyEmailComponent
      },
      {
        path: '',
        redirectTo: 'login',
        pathMatch: 'full'
      }
    ]
  },
  {
    path: 'admin',
    component: AdminLayoutComponent,
    children: [
      {
        path: 'dashboard',
        component: DashboardComponent
      },
      {
        path: 'notifications',
        component: NotificationsComponent
      },
      {
        path: 'users',
        component: UsersComponent
      },
      {
        path: 'specialties',
        loadComponent: () => import('./pages/admin/specialties/specialties').then(m => m.SpecialtiesComponent)
      },
      {
        path: 'profile',
        loadComponent: () => import('./pages/admin/profile/profile').then(m => m.ProfileComponent)
      },
      {
        path: 'audit-logs',
        loadComponent: () => import('./pages/admin/audit-logs/audit-logs').then(m => m.AuditLogsComponent)
      },
      {
        path: 'reports',
        loadComponent: () => import('./pages/admin/reports/reports').then(m => m.ReportsComponent)
      },
      {
        path: 'invites',
        loadComponent: () => import('./pages/admin/invites/invites').then(m => m.InvitesComponent)
      },
      {
        path: 'schedules',
        loadComponent: () => import('./pages/admin/schedules').then(m => m.SchedulesComponent)
      },
      {
        path: 'schedule-blocks',
        loadComponent: () => import('./pages/admin/schedule-blocks').then(m => m.ScheduleBlocksComponent)
      },
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      }
    ]
  },
  {
    path: '**',
    redirectTo: ''
  }
];
