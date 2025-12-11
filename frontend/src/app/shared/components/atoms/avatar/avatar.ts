import { Component, Input } from '@angular/core';
import { NgClass } from '@angular/common';

export type AvatarSize = 'sm' | 'md' | 'lg' | 'xl' | '2xl';

@Component({
  selector: 'app-avatar',
  imports: [NgClass],
  templateUrl: './avatar.html',
  styleUrl: './avatar.scss'
})
export class AvatarComponent {
  @Input() name: string = '';
  @Input() imageUrl?: string;
  @Input() size: AvatarSize = 'md';

  get initials(): string {
    if (!this.name) return '';
    
    const parts = this.name.trim().split(' ');
    if (parts.length === 1) {
      return parts[0].substring(0, 2).toUpperCase();
    }
    
    return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
  }

  get sizeClass(): string {
    return `avatar--${this.size}`;
  }
}
