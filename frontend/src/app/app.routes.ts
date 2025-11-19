import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./test.component').then(m => m.TestComponent)
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
