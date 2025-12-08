import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-specialties',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="specialties-container">
      <h3>Gerenciar Especialidades</h3>
      <p *ngIf="embeddedMode">Modo Embedded</p>
      <!-- TODO: Implementar gerenciamento de especialidades -->
    </div>
  `,
  styles: [`
    .specialties-container {
      padding: 1rem;
    }
  `]
})
export class SpecialtiesComponent {
  @Input() embeddedMode: boolean = false;
}
