import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet],
  template: `
    <div class="min-h-screen bg-gray-50">
      <!-- Header -->
      <header class="bg-white shadow-sm sticky top-0 z-50">
        <nav class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div class="flex justify-between h-16">
            <div class="flex">
              <!-- Logo -->
              <div class="flex-shrink-0 flex items-center">
                <h1 class="text-2xl font-bold text-primary-600">TeleCuidar</h1>
              </div>
              
              <!-- Navigation -->
              <div class="hidden sm:ml-6 sm:flex sm:space-x-8">
                <a href="#" class="border-primary-500 text-gray-900 inline-flex items-center px-1 pt-1 border-b-2 text-sm font-medium">
                  Dashboard
                </a>
                <a href="#" class="border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700 inline-flex items-center px-1 pt-1 border-b-2 text-sm font-medium">
                  Pacientes
                </a>
                <a href="#" class="border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700 inline-flex items-center px-1 pt-1 border-b-2 text-sm font-medium">
                  Consultas
                </a>
              </div>
            </div>
            
            <!-- User Menu -->
            <div class="flex items-center">
              <button class="bg-white p-1 rounded-full text-gray-400 hover:text-gray-500">
                <span class="sr-only">Notificações</span>
                🔔
              </button>
              <div class="ml-3 relative">
                <button class="bg-white rounded-full flex text-sm focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500">
                  <span class="sr-only">Menu do usuário</span>
                  <div class="h-8 w-8 rounded-full bg-primary-600 flex items-center justify-center text-white">
                    U
                  </div>
                </button>
              </div>
            </div>
          </div>
        </nav>
      </header>

      <!-- Main Content -->
      <main class="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
        <router-outlet></router-outlet>
      </main>

      <!-- Footer -->
      <footer class="bg-white border-t border-gray-200 mt-auto">
        <div class="max-w-7xl mx-auto py-4 px-4 sm:px-6 lg:px-8">
          <p class="text-center text-sm text-gray-500">
            © 2025 TeleCuidar. Plataforma de Telesaúde Pública.
          </p>
        </div>
      </footer>
    </div>
  `,
  styles: [`
    :host {
      display: block;
      min-height: 100vh;
    }
  `]
})
export class MainLayoutComponent {}
