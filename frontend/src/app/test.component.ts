import { Component } from '@angular/core';

@Component({
  selector: 'app-test',
  standalone: true,
  template: `
    <div style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); min-height: 100vh; display: flex; align-items: center; justify-content: center; color: white; text-align: center; font-family: Arial, sans-serif;">
      <div>
        <h1 style="font-size: 4rem; margin-bottom: 1rem; text-shadow: 2px 2px 4px rgba(0,0,0,0.3);">🚀 app</h1>
        <p style="font-size: 1.5rem; margin-bottom: 2rem; opacity: 0.9;">Sistema Web Moderno com C# .NET + Angular</p>
        <button style="background: rgba(255,255,255,0.2); border: 2px solid white; color: white; padding: 15px 30px; border-radius: 50px; font-size: 1.1rem; cursor: pointer; backdrop-filter: blur(10px); transition: all 0.3s ease;">
          🎯 Começar Agora
        </button>
      </div>
    </div>
  `
})
export class TestComponent {
}