import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { LogoComponent } from '../../atoms/logo/logo';
import { ButtonComponent } from '../../atoms/button/button';
import { ThemeToggleComponent } from '../../atoms/theme-toggle/theme-toggle';
import { IconComponent } from '../../atoms/icon/icon';

@Component({
  selector: 'app-header',
  imports: [CommonModule, RouterLink, LogoComponent, ButtonComponent, ThemeToggleComponent, IconComponent],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class HeaderComponent {
  isMenuOpen = false;

  toggleMenu() {
    this.isMenuOpen = !this.isMenuOpen;
  }
}
