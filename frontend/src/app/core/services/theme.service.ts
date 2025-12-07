import { Injectable, signal } from '@angular/core';

export type Theme = 'light' | 'dark';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private readonly THEME_KEY = 'app-theme';
  
  // Signal para reatividade
  currentTheme = signal<Theme>('light');

  constructor() {
    this.loadTheme();
  }

  /**
   * Carrega o tema salvo ou detecta preferência do sistema
   */
  private loadTheme(): void {
    const savedTheme = localStorage.getItem(this.THEME_KEY) as Theme;
    
    if (savedTheme) {
      this.setTheme(savedTheme);
    } else {
      // Detecta preferência do sistema
      const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
      this.setTheme(prefersDark ? 'dark' : 'light');
    }

    // Escuta mudanças na preferência do sistema
    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
      if (!localStorage.getItem(this.THEME_KEY)) {
        this.setTheme(e.matches ? 'dark' : 'light');
      }
    });
  }

  /**
   * Define o tema e aplica no documento
   */
  setTheme(theme: Theme): void {
    this.currentTheme.set(theme);
    
    // Aplica atributo data-theme para CSS variables
    document.documentElement.setAttribute('data-theme', theme);
    
    // Aplica classe 'dark' para suporte adicional
    if (theme === 'dark') {
      document.documentElement.classList.add('dark');
    } else {
      document.documentElement.classList.remove('dark');
    }
    
    localStorage.setItem(this.THEME_KEY, theme);
  }

  /**
   * Alterna entre light e dark
   */
  toggleTheme(): void {
    const newTheme = this.currentTheme() === 'light' ? 'dark' : 'light';
    this.setTheme(newTheme);
  }

  /**
   * Verifica se está no modo escuro
   */
  isDarkMode(): boolean {
    return this.currentTheme() === 'dark';
  }
}
