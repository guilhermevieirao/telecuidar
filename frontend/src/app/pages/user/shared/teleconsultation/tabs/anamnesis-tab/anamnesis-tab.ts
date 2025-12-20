import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { IconComponent } from '@shared/components/atoms/icon/icon';
import { ButtonComponent } from '@shared/components/atoms/button/button';
import { Subject, takeUntil, debounceTime } from 'rxjs';

@Component({
  selector: 'app-anamnesis-tab',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, IconComponent, ButtonComponent],
  templateUrl: './anamnesis-tab.html',
  styleUrls: ['./anamnesis-tab.scss']
})
export class AnamnesisTabComponent implements OnInit, OnDestroy {
  @Input() appointmentId: string | null = null;
  @Input() userrole: 'PATIENT' | 'PROFESSIONAL' | 'ADMIN' = 'PROFESSIONAL';
  @Input() readonly = false;

  anamnesisForm: FormGroup;
  isSaving = false;
  lastSaved: Date | null = null;
  private destroy$ = new Subject<void>();

  constructor(private fb: FormBuilder) {
    this.anamnesisForm = this.fb.group({
      // Queixa Principal
      chiefComplaint: [''],
      
      // História da Doença Atual
      presentIllnessHistory: [''],
      
      // Antecedentes Pessoais
      personalHistory: this.fb.group({
        previousDiseases: [''],
        surgeries: [''],
        hospitalizations: [''],
        allergies: [''],
        currentMedications: [''],
        vaccinations: ['']
      }),
      
      // Antecedentes Familiares
      familyHistory: [''],
      
      // Hábitos de Vida
      lifestyle: this.fb.group({
        diet: [''],
        physicalActivity: [''],
        smoking: [''],
        alcohol: [''],
        drugs: [''],
        sleep: ['']
      }),
      
      // Revisão de Sistemas
      systemsReview: this.fb.group({
        cardiovascular: [''],
        respiratory: [''],
        gastrointestinal: [''],
        genitourinary: [''],
        musculoskeletal: [''],
        neurological: [''],
        psychiatric: [''],
        endocrine: [''],
        hematologic: ['']
      }),
      
      // Observações Adicionais
      additionalNotes: ['']
    });
  }

  ngOnInit() {
    this.loadAnamnesisData();
    
    // Desabilitar para pacientes ou modo readonly (somente visualização)
    if (this.userrole === 'PATIENT' || this.readonly) {
      this.anamnesisForm.disable();
      return;
    }
    
    // Auto-save on value changes (debounced) - apenas para profissionais
    this.anamnesisForm.valueChanges
      .pipe(
        takeUntil(this.destroy$),
        debounceTime(2000)
      )
      .subscribe(() => {
        if (this.anamnesisForm.dirty) {
          this.saveAnamnesis();
        }
      });
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadAnamnesisData() {
    // TODO: Carregar dados da anamnese do backend
    // Por enquanto, deixar em branco para preenchimento
  }

  saveAnamnesis() {
    if (this.anamnesisForm.invalid) return;

    this.isSaving = true;
    
    // TODO: Salvar no backend
    // Por enquanto, simular salvamento
    setTimeout(() => {
      this.isSaving = false;
      this.lastSaved = new Date();
      this.anamnesisForm.markAsPristine();
    }, 1000);
  }

  onManualSave() {
    this.saveAnamnesis();
  }
}
