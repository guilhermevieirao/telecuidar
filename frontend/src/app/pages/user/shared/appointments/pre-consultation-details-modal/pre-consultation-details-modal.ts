import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonComponent } from '@shared/components/atoms/button/button';
import { IconComponent } from '@shared/components/atoms/icon/icon';
import { PreConsultationForm } from '@core/services/appointments.service';

@Component({
  selector: 'app-pre-consultation-details-modal',
  standalone: true,
  imports: [CommonModule, ButtonComponent, IconComponent],
  templateUrl: './pre-consultation-details-modal.html',
  styleUrls: ['./pre-consultation-details-modal.scss']
})
export class PreConsultationDetailsModalComponent {
  @Input() isOpen = false;
  @Input() preConsultation: PreConsultationForm | undefined;
  @Output() close = new EventEmitter<void>();

  onClose() {
    this.close.emit();
  }
}
