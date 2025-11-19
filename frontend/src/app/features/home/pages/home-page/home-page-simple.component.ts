import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 flex items-center justify-center">
      <div class="text-center">
        <h1 class="text-6xl font-bold text-gray-900 mb-4">
          Bem-vindo à <span class="text-blue-600">app</span>
        </h1>
        <p class="text-xl text-gray-600 mb-8">
          Sistema web moderno com C# .NET e Angular
        </p>
        <button class="bg-blue-600 text-white px-8 py-3 rounded-lg hover:bg-blue-700 transition-colors">
          Começar Agora
        </button>
      </div>
    </div>
  `
})
export class HomePageComponent {
}