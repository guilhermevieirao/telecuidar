import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-auth-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet],
  template: `
    <div class="min-h-screen flex flex-col bg-gradient-to-br from-primary-50 to-primary-100">
      <!-- Header Simples -->
      <header class="py-6">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <h1 class="text-3xl font-bold text-primary-600">TeleCuidar</h1>
        </div>
      </header>

      <!-- Main Content -->
      <main class="flex-grow flex items-center justify-center px-4 sm:px-6 lg:px-8">
        <div class="max-w-md w-full">
          <router-outlet></router-outlet>
        </div>
      </main>

      <!-- Footer -->
      <footer class="py-6">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <p class="text-center text-sm text-gray-600">
            © 2025 TeleCuidar. Todos os direitos reservados.
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
export class AuthLayoutComponent {}
