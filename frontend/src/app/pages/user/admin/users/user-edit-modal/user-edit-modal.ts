import { Component, EventEmitter, Input, Output, OnChanges, SimpleChanges, OnInit, inject, afterNextRender, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { IconComponent } from '@app/shared/components/atoms/icon/icon';
import { ButtonComponent } from '@app/shared/components/atoms/button/button';
import { AvatarComponent } from '@app/shared/components/atoms/avatar/avatar';
import { CpfMaskDirective } from '@app/core/directives/cpf-mask.directive';
import { PhoneMaskDirective } from '@app/core/directives/phone-mask.directive';
import { EmailValidatorDirective } from '@app/core/directives/email-validator.directive';
import { Usuario, TipoUsuario, StatusUsuario } from '@app/core/services/users.service';
import { SpecialtiesService, Specialty } from '@app/core/services/specialties.service';

@Component({
  selector: 'app-user-edit-modal',
  imports: [FormsModule, IconComponent, ButtonComponent, AvatarComponent, CpfMaskDirective, PhoneMaskDirective, EmailValidatorDirective],
  templateUrl: './user-edit-modal.html',
  styleUrl: './user-edit-modal.scss'
})
export class UserEditModalComponent implements OnChanges, OnInit {
  @Input() user: Usuario | null = null;
  @Input() isOpen = false;
  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<Usuario>();

  editedUser: Partial<Usuario> = {};
  specialties: Specialty[] = [];
  private cdr = inject(ChangeDetectorRef);

  constructor(private specialtiesService: SpecialtiesService) {
    afterNextRender(() => {
      this.loadSpecialties();
    });
  }

  ngOnInit(): void {}

  loadSpecialties(): void {
    this.specialtiesService.getSpecialties({ status: 'Ativo' }, { field: 'nome', direction: 'asc' }, 1, 100).subscribe({
      next: (response) => {
        this.specialties = response.dados;
        this.cdr.detectChanges();
      }
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['user'] && this.user) {
      this.editedUser = { ...this.user };
      // Detectar mudanças após atualizar dados para evitar NG0100
      setTimeout(() => this.cdr.detectChanges(), 0);
    }
  }

  onBackdropClick(): void {
    this.onCancel();
  }

  onCancel(): void {
    this.close.emit();
  }

  onSpecialtyChange(specialtyId: string): void {
    if (!this.editedUser.perfilProfissional) {
      this.editedUser.perfilProfissional = {};
    }
    this.editedUser.perfilProfissional.especialidadeId = specialtyId || undefined;
  }

  onSave(): void {
    if (this.editedUser && this.isFormValid()) {
      this.save.emit(this.editedUser as Usuario);
    }
  }

  isFormValid(): boolean {
    return !!(
      this.editedUser.nome?.trim() &&
      this.editedUser.email?.trim() &&
      this.editedUser.cpf?.trim() &&
      this.editedUser.telefone?.trim() &&
      this.editedUser.tipo &&
      this.editedUser.status
    );
  }

  getRoleLabel(role: TipoUsuario): string {
    const labels: Record<TipoUsuario, string> = {
      Paciente: 'Paciente',
      Profissional: 'Profissional',
      Administrador: 'Administrador'
    };
    return labels[role];
  }

  getStatusLabel(status: StatusUsuario): string {
    const labels: Record<StatusUsuario, string> = {
      Ativo: 'Ativo',
      Inativo: 'Inativo'
    };
    return labels[status];
  }
}
