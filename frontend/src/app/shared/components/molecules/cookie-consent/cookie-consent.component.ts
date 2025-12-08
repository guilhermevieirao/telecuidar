import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-cookie-consent',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './cookie-consent.component.html',
  styleUrls: ['./cookie-consent.component.scss']
})
export class CookieConsentComponent {
  showBanner = signal(false);

  constructor() {
    // Verifica se o usuário já aceitou/rejeitou cookies
    const consent = localStorage.getItem('cookie-consent');
    if (!consent) {
      // Mostra o banner após 1 segundo
      setTimeout(() => {
        this.showBanner.set(true);
      }, 1000);
    }
  }

  acceptCookies(): void {
    localStorage.setItem('cookie-consent', 'accepted');
    localStorage.setItem('cookie-consent-date', new Date().toISOString());
    this.showBanner.set(false);
  }

  rejectCookies(): void {
    localStorage.setItem('cookie-consent', 'rejected');
    localStorage.setItem('cookie-consent-date', new Date().toISOString());
    this.showBanner.set(false);
    
    // Remove cookies não essenciais
    this.clearNonEssentialCookies();
  }

  private clearNonEssentialCookies(): void {
    // Mantém apenas cookies essenciais (auth token, theme)
    const essentialKeys = ['token', 'user', 'theme'];
    const allKeys = Object.keys(localStorage);
    
    allKeys.forEach(key => {
      if (!essentialKeys.includes(key) && !key.startsWith('cookie-consent')) {
        localStorage.removeItem(key);
      }
    });
  }
}
