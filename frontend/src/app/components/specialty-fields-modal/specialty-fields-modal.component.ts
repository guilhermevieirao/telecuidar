import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SpecialtyService } from '../../services/specialty.service';
import { 
  SpecialtyFieldDto, 
  CreateSpecialtyFieldDto,
  UpdateSpecialtyFieldDto 
} from '../../models/specialty.model';

@Component({
  selector: 'app-specialty-fields-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="modal-overlay" (click)="onClose()">
      <div class="modal-content" (click)="$event.stopPropagation()">
        <div class="modal-header">
          <h2>Campos Personalizados - {{ specialtyName }}</h2>
          <button class="btn-close" (click)="onClose()">✕</button>
        </div>

        <div class="modal-body">
          <!-- Lista de campos existentes -->
          <div class="fields-list" *ngIf="fields.length > 0">
            <h3>Campos Cadastrados</h3>
            <div class="field-item" *ngFor="let field of fields">
              <div class="field-info">
                <strong>{{ field.label }}</strong>
                <span class="field-type">{{ getFieldTypeLabel(field.fieldType) }}</span>
                <span class="field-required" *ngIf="field.isRequired">*Obrigatório</span>
              </div>
              <div class="field-actions">
                <button class="btn-edit" (click)="editField(field)">✏️</button>
                <button class="btn-delete" (click)="deleteField(field.id)">🗑️</button>
              </div>
            </div>
          </div>

          <div *ngIf="fields.length === 0" class="empty-state">
            <p>Nenhum campo personalizado cadastrado</p>
          </div>

          <!-- Formulário de criar/editar campo -->
          <div class="field-form" *ngIf="showForm">
            <h3>{{ editingField ? 'Editar Campo' : 'Novo Campo' }}</h3>
            
            <div class="form-group">
              <label>Nome do Campo (ID)*</label>
              <input 
                type="text" 
                [(ngModel)]="formData.fieldName"
                placeholder="ex: pressao_arterial"
                [disabled]="!!editingField"
              />
            </div>

            <div class="form-group">
              <label>Rótulo*</label>
              <input 
                type="text" 
                [(ngModel)]="formData.label"
                placeholder="ex: Pressão Arterial"
              />
            </div>

            <div class="form-group">
              <label>Descrição</label>
              <textarea 
                [(ngModel)]="formData.description"
                placeholder="Descrição do campo (opcional)"
                rows="2"
              ></textarea>
            </div>

            <div class="form-group">
              <label>Tipo de Campo*</label>
              <select [(ngModel)]="formData.fieldType">
                <option value="text">Texto Simples</option>
                <option value="textarea">Texto Longo</option>
                <option value="number">Número</option>
                <option value="date">Data</option>
                <option value="select">Seleção (Dropdown)</option>
                <option value="checkbox">Checkbox</option>
                <option value="radio">Opções (Radio)</option>
              </select>
            </div>

            <div class="form-group" *ngIf="formData.fieldType === 'select' || formData.fieldType === 'radio'">
              <label>Opções (uma por linha)*</label>
              <textarea 
                [(ngModel)]="optionsText"
                placeholder="Opção 1&#10;Opção 2&#10;Opção 3"
                rows="4"
              ></textarea>
            </div>

            <div class="form-group">
              <label>Placeholder</label>
              <input 
                type="text" 
                [(ngModel)]="formData.placeholder"
                placeholder="Texto de exemplo no campo"
              />
            </div>

            <div class="form-group">
              <label>Valor Padrão</label>
              <input 
                type="text" 
                [(ngModel)]="formData.defaultValue"
                placeholder="Valor inicial (opcional)"
              />
            </div>

            <div class="form-group">
              <label>Ordem de Exibição*</label>
              <input 
                type="number" 
                [(ngModel)]="formData.displayOrder"
                min="1"
              />
            </div>

            <div class="form-group checkbox-group">
              <label>
                <input 
                  type="checkbox" 
                  [(ngModel)]="formData.isRequired"
                />
                Campo obrigatório
              </label>
            </div>

            <div class="form-actions">
              <button class="btn-cancel" (click)="cancelForm()">Cancelar</button>
              <button class="btn-save" (click)="saveField()">
                {{ editingField ? 'Atualizar' : 'Criar' }}
              </button>
            </div>
          </div>

          <button class="btn-add-field" *ngIf="!showForm" (click)="showForm = true">
            + Adicionar Campo
          </button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .modal-overlay {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background: rgba(0, 0, 0, 0.7);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 1000;
      padding: 20px;
    }

    .modal-content {
      background: white;
      border-radius: 12px;
      width: 100%;
      max-width: 700px;
      max-height: 90vh;
      display: flex;
      flex-direction: column;
      box-shadow: 0 10px 40px rgba(0, 0, 0, 0.3);
    }

    .modal-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 24px;
      border-bottom: 1px solid #e5e7eb;
    }

    .modal-header h2 {
      margin: 0;
      font-size: 1.5rem;
      color: #1f2937;
    }

    .btn-close {
      background: none;
      border: none;
      font-size: 1.5rem;
      cursor: pointer;
      color: #6b7280;
      padding: 4px 8px;
      transition: color 0.2s;
    }

    .btn-close:hover {
      color: #1f2937;
    }

    .modal-body {
      padding: 24px;
      overflow-y: auto;
    }

    .fields-list {
      margin-bottom: 24px;
    }

    .fields-list h3 {
      margin: 0 0 16px 0;
      font-size: 1.125rem;
      color: #374151;
    }

    .field-item {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 12px 16px;
      background: #f9fafb;
      border: 1px solid #e5e7eb;
      border-radius: 8px;
      margin-bottom: 8px;
    }

    .field-info {
      display: flex;
      flex-direction: column;
      gap: 4px;
    }

    .field-info strong {
      color: #1f2937;
      font-size: 1rem;
    }

    .field-type {
      font-size: 0.875rem;
      color: #6b7280;
    }

    .field-required {
      font-size: 0.75rem;
      color: #ef4444;
      font-weight: 500;
    }

    .field-actions {
      display: flex;
      gap: 8px;
    }

    .btn-edit, .btn-delete {
      background: none;
      border: none;
      font-size: 1.25rem;
      cursor: pointer;
      padding: 4px 8px;
      transition: transform 0.2s;
    }

    .btn-edit:hover, .btn-delete:hover {
      transform: scale(1.2);
    }

    .empty-state {
      text-align: center;
      padding: 40px 20px;
      color: #6b7280;
    }

    .field-form {
      background: #f9fafb;
      border: 2px dashed #d1d5db;
      border-radius: 8px;
      padding: 20px;
      margin-top: 20px;
    }

    .field-form h3 {
      margin: 0 0 20px 0;
      font-size: 1.125rem;
      color: #374151;
    }

    .form-group {
      margin-bottom: 16px;
    }

    .form-group label {
      display: block;
      margin-bottom: 6px;
      font-weight: 500;
      color: #374151;
      font-size: 0.875rem;
    }

    .form-group input,
    .form-group textarea,
    .form-group select {
      width: 100%;
      padding: 10px 12px;
      border: 1px solid #d1d5db;
      border-radius: 6px;
      font-size: 0.875rem;
      transition: border-color 0.2s;
    }

    .form-group input:focus,
    .form-group textarea:focus,
    .form-group select:focus {
      outline: none;
      border-color: #3b82f6;
    }

    .form-group textarea {
      resize: vertical;
      font-family: inherit;
    }

    .checkbox-group label {
      display: flex;
      align-items: center;
      gap: 8px;
      cursor: pointer;
    }

    .checkbox-group input[type="checkbox"] {
      width: auto;
      cursor: pointer;
    }

    .form-actions {
      display: flex;
      gap: 12px;
      margin-top: 20px;
    }

    .btn-cancel,
    .btn-save {
      flex: 1;
      padding: 10px 20px;
      border: none;
      border-radius: 6px;
      font-weight: 500;
      cursor: pointer;
      transition: all 0.2s;
    }

    .btn-cancel {
      background: #f3f4f6;
      color: #374151;
    }

    .btn-cancel:hover {
      background: #e5e7eb;
    }

    .btn-save {
      background: #3b82f6;
      color: white;
    }

    .btn-save:hover {
      background: #2563eb;
    }

    .btn-add-field {
      width: 100%;
      padding: 12px;
      background: #3b82f6;
      color: white;
      border: none;
      border-radius: 6px;
      font-weight: 500;
      cursor: pointer;
      transition: background 0.2s;
      margin-top: 16px;
    }

    .btn-add-field:hover {
      background: #2563eb;
    }
  `]
})
export class SpecialtyFieldsModalComponent implements OnInit {
  @Input() specialtyId!: number;
  @Input() specialtyName!: string;
  @Output() close = new EventEmitter<void>();

  fields: SpecialtyFieldDto[] = [];
  showForm = false;
  editingField: SpecialtyFieldDto | null = null;
  optionsText = '';

  formData: CreateSpecialtyFieldDto = {
    fieldName: '',
    label: '',
    description: '',
    fieldType: 'text',
    options: [],
    isRequired: false,
    displayOrder: 1,
    defaultValue: '',
    placeholder: ''
  };

  constructor(private specialtyService: SpecialtyService) {}

  ngOnInit(): void {
    this.loadFields();
  }

  loadFields(): void {
    this.specialtyService.getFields(this.specialtyId).subscribe({
      next: (fields) => {
        this.fields = fields.sort((a, b) => a.displayOrder - b.displayOrder);
      },
      error: (error) => {
        console.error('Erro ao carregar campos:', error);
      }
    });
  }

  getFieldTypeLabel(type: string): string {
    const labels: { [key: string]: string } = {
      'text': 'Texto',
      'textarea': 'Texto Longo',
      'number': 'Número',
      'date': 'Data',
      'select': 'Seleção',
      'checkbox': 'Checkbox',
      'radio': 'Opções'
    };
    return labels[type] || type;
  }

  editField(field: SpecialtyFieldDto): void {
    this.editingField = field;
    this.formData = {
      fieldName: field.fieldName,
      label: field.label,
      description: field.description,
      fieldType: field.fieldType,
      options: field.options || [],
      isRequired: field.isRequired,
      displayOrder: field.displayOrder,
      defaultValue: field.defaultValue,
      placeholder: field.placeholder
    };
    this.optionsText = field.options ? field.options.join('\n') : '';
    this.showForm = true;
  }

  deleteField(fieldId: number): void {
    if (confirm('Tem certeza que deseja excluir este campo?')) {
      this.specialtyService.deleteField(fieldId).subscribe({
        next: () => {
          this.loadFields();
        },
        error: (error) => {
          console.error('Erro ao excluir campo:', error);
          alert('Erro ao excluir campo');
        }
      });
    }
  }

  saveField(): void {
    // Processar opções se for select ou radio
    if (this.formData.fieldType === 'select' || this.formData.fieldType === 'radio') {
      this.formData.options = this.optionsText
        .split('\n')
        .map(opt => opt.trim())
        .filter(opt => opt.length > 0);
    } else {
      this.formData.options = undefined;
    }

    if (!this.formData.fieldName || !this.formData.label) {
      alert('Preencha os campos obrigatórios');
      return;
    }

    if (this.editingField) {
      // Atualizar campo existente
      const updateData: UpdateSpecialtyFieldDto = { ...this.formData };
      this.specialtyService.updateField(this.editingField.id, updateData).subscribe({
        next: () => {
          this.loadFields();
          this.cancelForm();
        },
        error: (error) => {
          console.error('Erro ao atualizar campo:', error);
          alert('Erro ao atualizar campo');
        }
      });
    } else {
      // Criar novo campo
      this.specialtyService.createField(this.specialtyId, this.formData).subscribe({
        next: () => {
          this.loadFields();
          this.cancelForm();
        },
        error: (error) => {
          console.error('Erro ao criar campo:', error);
          alert('Erro ao criar campo');
        }
      });
    }
  }

  cancelForm(): void {
    this.showForm = false;
    this.editingField = null;
    this.formData = {
      fieldName: '',
      label: '',
      description: '',
      fieldType: 'text',
      options: [],
      isRequired: false,
      displayOrder: this.fields.length + 1,
      defaultValue: '',
      placeholder: ''
    };
    this.optionsText = '';
  }

  onClose(): void {
    this.close.emit();
  }
}
