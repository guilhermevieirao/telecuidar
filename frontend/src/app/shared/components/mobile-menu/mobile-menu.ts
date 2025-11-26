import { Component, Input, Output, EventEmitter, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

export interface MenuItem {
  label?: string;
  route?: string;
  action?: () => void;
  icon?: string;
  divider?: boolean;
}

@Component({
  selector: 'app-mobile-menu',
  imports: [CommonModule, RouterLink],
  templateUrl: './mobile-menu.html',
  styleUrl: './mobile-menu.scss',
})
export class MobileMenu {
  @Input() items: MenuItem[] = [];
  @Input() userName?: string;
  @Input() userRole?: string;
  @Input() userAvatar?: string;
  @Output() itemClick = new EventEmitter<MenuItem>();
  
  isOpen = signal(false);

  toggle() {
    this.isOpen.set(!this.isOpen());
  }

  close() {
    this.isOpen.set(false);
  }

  handleItemClick(item: MenuItem) {
    if (item.action) {
      item.action();
    }
    this.itemClick.emit(item);
    this.close();
  }
}
