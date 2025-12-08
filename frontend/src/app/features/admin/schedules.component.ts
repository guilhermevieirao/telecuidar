import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-schedules',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="schedules-container">
      <h3>Gerenciar Agendas</h3>
      <p *ngIf="embeddedMode">Modo Embedded</p>
      <!-- TODO: Implementar gerenciamento de agendas -->
    </div>
  `,
  styles: [`
    .schedules-container {
      padding: 1rem;
    }
  `]
})
export class SchedulesComponent {
  @Input() embeddedMode: boolean = false;
}
