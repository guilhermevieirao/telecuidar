import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { ToastComponent } from './shared/components/molecules/toast/toast.component';
import { CookieConsentComponent } from './shared/components/molecules/cookie-consent/cookie-consent.component';
import { ModalManagerComponent } from './shared/components/organisms/modal-manager/modal-manager.component';
import { ThemeService } from './core/services/theme.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, ToastComponent, CookieConsentComponent, ModalManagerComponent],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'TeleCuidar';
  
  // Injeta ThemeService para inicializar na inicialização da app
  private themeService = inject(ThemeService);
}
