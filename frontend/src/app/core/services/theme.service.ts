import { Injectable, Renderer2, RendererFactory2, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  private renderer: Renderer2;
  private readonly THEME_KEY = 'telecuidar-theme';

  constructor(
    rendererFactory: RendererFactory2,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    this.renderer = rendererFactory.createRenderer(null, null);
    this.initTheme();
  }

  private initTheme(): void {
    if (isPlatformBrowser(this.platformId)) {
      const savedTheme = localStorage.getItem(this.THEME_KEY);
      const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
      const theme = savedTheme || (prefersDark ? 'dark' : 'light');
      this.setTheme(theme);
    }
  }

  private setTheme(theme: string): void {
    if (isPlatformBrowser(this.platformId)) {
      if (theme === 'dark') {
        this.renderer.setAttribute(document.documentElement, 'data-theme', 'dark');
      } else {
        this.renderer.removeAttribute(document.documentElement, 'data-theme');
      }
      localStorage.setItem(this.THEME_KEY, theme);
    }
  }

  toggleTheme(): void {
    const currentTheme = this.isDarkMode() ? 'dark' : 'light';
    const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
    this.setTheme(newTheme);
  }

  isDarkMode(): boolean {
    if (isPlatformBrowser(this.platformId)) {
      return document.documentElement.getAttribute('data-theme') === 'dark';
    }
    return false;
  }
}
