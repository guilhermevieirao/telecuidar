import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';

const API_BASE_URL = environment.apiUrl;

export type TipoUsuario = 'Paciente' | 'Profissional' | 'Administrador';
export type StatusUsuario = 'Ativo' | 'Inativo';

// Aliases para compatibilidade
export type UserRole = TipoUsuario;
export type UserStatus = StatusUsuario;

// ============================================
// Interfaces de Perfis Específicos
// ============================================

export interface PerfilPaciente {
  id?: string;
  cns?: string;
  nomeSocial?: string;
  sexo?: string;
  dataNascimento?: string;
  nomeMae?: string;
  nomePai?: string;
  nacionalidade?: string;
  cep?: string;
  endereco?: string;
  cidade?: string;
  estado?: string;
}

export interface PerfilProfissional {
  id?: string;
  crm?: string;
  cbo?: string;
  especialidadeId?: string;
  nomeEspecialidade?: string;
  sexo?: string;
  dataNascimento?: string;
  nacionalidade?: string;
  cep?: string;
  endereco?: string;
  cidade?: string;
  estado?: string;
}

// Aliases para compatibilidade
export type PatientProfile = PerfilPaciente;
export type ProfessionalProfile = PerfilProfissional;

export interface CriarAtualizarPerfilPaciente {
  cns?: string;
  nomeSocial?: string;
  sexo?: string;
  dataNascimento?: string;
  nomeMae?: string;
  nomePai?: string;
  nacionalidade?: string;
  cep?: string;
  endereco?: string;
  cidade?: string;
  estado?: string;
}

export interface CriarAtualizarPerfilProfissional {
  crm?: string;
  cbo?: string;
  especialidadeId?: string;
  sexo?: string;
  dataNascimento?: string;
  nacionalidade?: string;
  cep?: string;
  endereco?: string;
  cidade?: string;
  estado?: string;
}

// Aliases para compatibilidade
export type CreateUpdatePatientProfile = CriarAtualizarPerfilPaciente;
export type CreateUpdateProfessionalProfile = CriarAtualizarPerfilProfissional;

// ============================================
// Interface de Usuário com Perfis
// ============================================

export interface Usuario {
  id: string;
  nome: string;
  sobrenome: string;
  email: string;
  tipo: TipoUsuario;
  cpf: string;
  telefone?: string;
  status: StatusUsuario;
  criadoEm: string;
  atualizadoEm?: string;
  avatar?: string;
  emailVerificado?: boolean;
  
  // Perfis específicos por tipo de usuário
  perfilPaciente?: PerfilPaciente;
  perfilProfissional?: PerfilProfissional;
}

// Alias para compatibilidade
export type User = Usuario;

export interface CriarUsuarioDto {
  nome: string;
  sobrenome: string;
  email: string;
  cpf: string;
  telefone?: string;
  senha: string;
  tipo: TipoUsuario;
  
  // Perfis específicos por tipo de usuário
  perfilPaciente?: CriarAtualizarPerfilPaciente;
  perfilProfissional?: CriarAtualizarPerfilProfissional;
}

// Alias para compatibilidade
export type CreateUserDto = CriarUsuarioDto;

export interface AtualizarUsuarioDto {
  nome?: string;
  sobrenome?: string;
  telefone?: string;
  avatar?: string;
  status?: StatusUsuario;
  tipo?: TipoUsuario;
  
  // Perfis específicos por tipo de usuário
  perfilPaciente?: CriarAtualizarPerfilPaciente;
  perfilProfissional?: CriarAtualizarPerfilProfissional;
}

// Alias para compatibilidade
export type UpdateUserDto = AtualizarUsuarioDto;

export interface UsersFilter {
  busca?: string;
  tipo?: string;
  status?: string;
  especialidadeId?: string;
}

// Alias para compatibilidade
export type UsersSortOptions = 'nome' | 'email' | 'criadoEm' | 'tipo' | 'status';

export interface RespostaPaginada<T> {
  dados: T[];
  total: number;
  pagina: number;
  tamanhoPagina: number;
  totalPaginas: number;
}

// Alias para compatibilidade
export type PaginatedResponse<T> = RespostaPaginada<T>;

@Injectable({
  providedIn: 'root'
})
export class UsersService {
  private apiUrl = `${API_BASE_URL}/usuarios`;

  constructor(private http: HttpClient) {}

  getUsers(
    filter?: UsersFilter,
    page: number = 1,
    pageSize: number = 10
  ): Observable<RespostaPaginada<Usuario>> {
    let params = new HttpParams()
      .set('pagina', page.toString())
      .set('tamanhoPagina', pageSize.toString());

    if (filter?.busca) {
      params = params.set('busca', filter.busca);
    }
    if (filter?.tipo) {
      params = params.set('tipo', filter.tipo);
    }
    if (filter?.status) {
      params = params.set('status', filter.status);
    }
    if (filter?.especialidadeId) {
      params = params.set('especialidadeId', filter.especialidadeId);
    }

    return this.http.get<RespostaPaginada<Usuario>>(this.apiUrl, { params });
  }

  getUserById(id: string): Observable<Usuario> {
    return this.http.get<Usuario>(`${this.apiUrl}/${id}`);
  }

  createUser(user: CriarUsuarioDto): Observable<Usuario> {
    return this.http.post<Usuario>(this.apiUrl, user);
  }

  updateUser(id: string, user: AtualizarUsuarioDto): Observable<Usuario> {
    return this.http.put<Usuario>(`${this.apiUrl}/${id}`, user);
  }

  deleteUser(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getUserStats(): Observable<any> {
    return this.http.get(`${this.apiUrl}/estatisticas`);
  }

  generateInviteLink(data: { email: string; tipo: TipoUsuario; especialidadeId?: string }): Observable<any> {
    return this.http.post(`${API_BASE_URL}/convites/gerar-link`, data);
  }

  sendInviteByEmail(data: { email: string; tipo: TipoUsuario; especialidadeId?: string }): Observable<any> {
    return this.http.post(`${API_BASE_URL}/convites/enviar-email`, data);
  }

  // ============================================
  // Métodos de Perfil de Paciente
  // ============================================

  getPatientProfile(userId: string): Observable<PerfilPaciente> {
    return this.http.get<PerfilPaciente>(`${this.apiUrl}/${userId}/perfil-paciente`);
  }

  updatePatientProfile(userId: string, profile: CriarAtualizarPerfilPaciente): Observable<PerfilPaciente> {
    return this.http.put<PerfilPaciente>(`${this.apiUrl}/${userId}/perfil-paciente`, profile);
  }

  // ============================================
  // Métodos de Perfil de Profissional
  // ============================================

  getProfessionalProfile(userId: string): Observable<PerfilProfissional> {
    return this.http.get<PerfilProfissional>(`${this.apiUrl}/${userId}/perfil-profissional`);
  }

  updateProfessionalProfile(userId: string, profile: CriarAtualizarPerfilProfissional): Observable<PerfilProfissional> {
    return this.http.put<PerfilProfissional>(`${this.apiUrl}/${userId}/perfil-profissional`, profile);
  }
}
