import { Component, EventEmitter, Input, Output, OnChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  role: number;
  roleName: string;
  emailConfirmed: boolean;
}

@Component({
  selector: 'app-edit-user-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="modal-overlay" *ngIf="show" (click)="onCancel()">
      <div class="modal-content" (click)="$event.stopPropagation()">
        <div class="modal-header">
          <h3>Editar Usuário</h3>
          <button class="close-btn" (click)="onCancel()">✕</button>
        </div>
        
        <div class="modal-body">
          <div class="form-group">
            <label for="firstName">Nome</label>
            <input
              type="text"
              id="firstName"
              [(ngModel)]="editData.firstName"
              placeholder="Nome"
            />
          </div>

          <div class="form-group">
            <label for="lastName">Sobrenome</label>
            <input
              type="text"
              id="lastName"
              [(ngModel)]="editData.lastName"
              placeholder="Sobrenome"
            />
          </div>

          <div class="form-group">
            <label for="email">E-mail</label>
            <input
              type="email"
              id="email"
              [(ngModel)]="editData.email"
              placeholder="E-mail"
            />
          </div>

          <div class="form-group">
            <label for="phoneNumber">Telefone</label>
            <input
              type="tel"
              id="phoneNumber"
              [(ngModel)]="editData.phoneNumber"
              placeholder="(11) 99999-9999"
            />
          </div>

          <div class="form-group">
            <label for="role">Perfil</label>
            <select id="role" [(ngModel)]="editData.role">
              <option [value]="1">Paciente</option>
              <option [value]="2">Profissional</option>
              <option [value]="3">Administrador</option>
            </select>
          </div>

          <div class="form-group checkbox">
            <label>
              <input
                type="checkbox"
                [(ngModel)]="editData.emailConfirmed"
              />
              <span>E-mail confirmado</span>
            </label>
          </div>
        </div>
        
        <div class="modal-footer">
          <button class="btn btn-secondary" (click)="onCancel()">Cancelar</button>
          <button class="btn btn-primary" (click)="onSave()">Salvar</button>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./edit-user-modal.component.scss']
})
export class EditUserModalComponent implements OnChanges {
  @Input() show: boolean = false;
  @Input() user: User | null = null;
  @Output() saved = new EventEmitter<any>();
  @Output() cancelled = new EventEmitter<void>();

  editData: any = {
    firstName: '',
    lastName: '',
    email: '',
    phoneNumber: '',
    role: 1,
    emailConfirmed: false
  };

  ngOnChanges() {
    if (this.user) {
      this.editData = {
        firstName: this.user.firstName,
        lastName: this.user.lastName,
        email: this.user.email,
        phoneNumber: this.user.phoneNumber || '',
        role: this.user.role,
        emailConfirmed: this.user.emailConfirmed
      };
    }
  }

  onSave() {
    this.saved.emit(this.editData);
  }

  onCancel() {
    this.cancelled.emit();
    this.show = false;
  }
}
