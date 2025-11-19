import { Component } from '@angular/core';

@Component({
  selector: 'app-home-page',
  standalone: true,
  template: `
    <div style="padding: 50px; text-align: center; font-family: Arial, sans-serif;">
      <h1 style="color: #3b82f6; font-size: 3rem; margin-bottom: 20px;">
        🚀 app
      </h1>
      <p style="font-size: 1.2rem; color: #666; margin-bottom: 30px;">
        Sistema web moderno com C# .NET + Angular + Tailwind CSS
      </p>
      <div style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 40px; border-radius: 20px; max-width: 600px; margin: 0 auto;">
        <h2 style="font-size: 2rem; margin-bottom: 15px;">Landing Page Funcionando!</h2>
        <p style="font-size: 1.1rem; opacity: 0.9;">
          Arquitetura limpa, design moderno e escalável prontos para produção.
        </p>
      </div>
    </div>
  `
})
export class HomePageComponent {
}