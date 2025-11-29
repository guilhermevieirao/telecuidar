import { Component, OnInit, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { SpecialtyService } from '../../services/specialty.service';
import { SpecialtyDto, CreateSpecialtyCommand, UpdateSpecialtyCommand } from '../../models/specialty.model';
import { environment } from '../../../environments/environment';

interface Professional {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  fullName: string;
  role: string;
}

@Component({
  selector: 'app-specialties',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './specialties.component.html',
  styleUrls: ['./specialties.component.scss']
})
export class SpecialtiesComponent implements OnInit {
  @Input() embeddedMode: boolean = false;

  specialties: SpecialtyDto[] = [];
  professionals: Professional[] = [];
  selectedSpecialty: SpecialtyDto | null = null;
  showCreateModal = false;
  showEditModal = false;
  showAssignModal = false;
  showUnassignModal = false;
  professionalsWithSpecialty: any[] = [];
  loading = false;

  newSpecialty: CreateSpecialtyCommand = {
    name: '',
    description: '',
    icon: ''
  };

  editSpecialty: UpdateSpecialtyCommand = {
    id: 0,
    name: '',
    description: '',
    icon: ''
  };

  constructor(
    private specialtyService: SpecialtyService,
    private http: HttpClient
  ) {}

  ngOnInit(): void {
    this.loadSpecialties();
  }

  loadSpecialties(): void {
    this.loading = true;
    this.specialtyService.getAll().subscribe({
      next: (data) => {
        this.specialties = data;
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar especialidades:', error);
        this.loading = false;
      }
    });
  }

  loadProfessionals(): void {
    // Role 2 = Profissional
    this.http.get<any>(`${environment.apiUrl}/users?role=2&pageSize=100`).subscribe({
      next: (response: any) => {
        console.log('Response:', response);
        if (response.isSuccess && response.data) {
          this.professionals = response.data.items || [];
          console.log('Profissionais carregados:', this.professionals);
        } else {
          this.professionals = [];
        }
      },
      error: (error: any) => {
        console.error('Erro ao carregar profissionais:', error);
        this.professionals = [];
      }
    });
  }

  openCreateModal(): void {
    this.newSpecialty = { name: '', description: '', icon: '' };
    this.showCreateModal = true;
  }

  closeCreateModal(): void {
    this.showCreateModal = false;
  }

  createSpecialty(): void {
    if (!this.newSpecialty.name.trim()) {
      alert('Nome da especialidade é obrigatório');
      return;
    }

    this.loading = true;
    this.specialtyService.create(this.newSpecialty).subscribe({
      next: () => {
        this.loadSpecialties();
        this.closeCreateModal();
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao criar especialidade:', error);
        alert('Erro ao criar especialidade');
        this.loading = false;
      }
    });
  }

  openEditModal(specialty: SpecialtyDto): void {
    this.editSpecialty = {
      id: specialty.id,
      name: specialty.name,
      description: specialty.description,
      icon: specialty.icon
    };
    this.showEditModal = true;
  }

  closeEditModal(): void {
    this.showEditModal = false;
  }

  updateSpecialty(): void {
    if (!this.editSpecialty.name.trim()) {
      alert('Nome da especialidade é obrigatório');
      return;
    }

    this.loading = true;
    this.specialtyService.update(this.editSpecialty).subscribe({
      next: () => {
        this.loadSpecialties();
        this.closeEditModal();
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao atualizar especialidade:', error);
        alert('Erro ao atualizar especialidade');
        this.loading = false;
      }
    });
  }

  deleteSpecialty(specialty: SpecialtyDto): void {
    if (!confirm(`Tem certeza que deseja excluir a especialidade "${specialty.name}"?`)) {
      return;
    }

    this.loading = true;
    this.specialtyService.delete(specialty.id).subscribe({
      next: () => {
        this.loadSpecialties();
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao excluir especialidade:', error);
        alert('Erro ao excluir especialidade');
        this.loading = false;
      }
    });
  }

  openAssignModal(specialty: SpecialtyDto): void {
    this.selectedSpecialty = specialty;
    this.loadProfessionals();
    this.showAssignModal = true;
  }

  closeAssignModal(): void {
    this.showAssignModal = false;
    this.selectedSpecialty = null;
  }

  openUnassignModal(specialty: SpecialtyDto): void {
    this.selectedSpecialty = specialty;
    this.loadProfessionalsWithSpecialty(specialty.id);
    this.showUnassignModal = true;
  }

  closeUnassignModal(): void {
    this.showUnassignModal = false;
    this.selectedSpecialty = null;
    this.professionalsWithSpecialty = [];
  }

  loadProfessionalsWithSpecialty(specialtyId: number): void {
    this.loading = true;
    this.http.get<any>(`${environment.apiUrl}/specialties/${specialtyId}/professionals`).subscribe({
      next: (response: any) => {
        console.log('Response professionals with specialty:', response);
        if (response.isSuccess && response.data) {
          this.professionalsWithSpecialty = response.data;
        } else {
          this.professionalsWithSpecialty = [];
        }
        console.log('Profissionais com especialidade:', this.professionalsWithSpecialty);
        this.loading = false;
      },
      error: (error: any) => {
        console.error('Erro ao carregar profissionais:', error);
        this.professionalsWithSpecialty = [];
        this.loading = false;
      }
    });
  }

  unassignFromProfessional(userId: number, specialtyId: number): void {
    if (!confirm('Tem certeza que deseja remover esta especialidade do profissional?')) {
      return;
    }

    this.loading = true;
    this.http.delete(`${environment.apiUrl}/specialties/${specialtyId}/professionals/${userId}`).subscribe({
      next: (response: any) => {
        alert('Especialidade removida com sucesso');
        this.loadProfessionalsWithSpecialty(specialtyId);
        this.loadSpecialties();
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao remover especialidade:', error);
        alert(error.error?.message || 'Erro ao remover especialidade');
        this.loading = false;
      }
    });
  }

  assignToProfessional(professionalId: number): void {
    if (!this.selectedSpecialty) return;

    this.loading = true;
    this.specialtyService.assignToProfessional(this.selectedSpecialty.id, professionalId).subscribe({
      next: () => {
        alert('Especialidade atribuída com sucesso');
        this.loadSpecialties();
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao atribuir especialidade:', error);
        alert(error.error?.message || 'Erro ao atribuir especialidade');
        this.loading = false;
      }
    });
  }
}
