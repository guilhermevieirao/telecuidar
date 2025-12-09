import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MobileMenu, MenuItem } from '../mobile-menu/mobile-menu';

@Component({
  selector: 'app-dashboard-navbar',
  standalone: true,
  imports: [CommonModule, MobileMenu],
  templateUrl: './dashboard-navbar.component.html',
  styleUrls: ['./dashboard-navbar.component.scss']
})
export class DashboardNavbarComponent {
  @Input() user: any;
  @Input() menuItems: MenuItem[] = [];
  @Input() getUserRoleName: () => string = () => '';
  @Input() getUserInitials: () => string = () => '';
  @Input() logout: () => void = () => {};
}
